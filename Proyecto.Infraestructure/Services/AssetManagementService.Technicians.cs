using Microsoft.EntityFrameworkCore;
using Proyecto.Application.DTO;
using Proyecto.Application.Mappers;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService
{
    public async Task<IReadOnlyList<TechnicianSummaryDto>> GetTechniciansAsync(
        int? specialtyId, bool? isAvailable, CancellationToken cancellationToken)
    {
        var query = _context.Technicians.AsNoTracking().AsQueryable();
        if (specialtyId.HasValue)
        {
            query = query.Where(technician => technician.Specialties.Any(relation => relation.SpecialtyId == specialtyId.Value));
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(technician => technician.IsAvailable == isAvailable.Value);
        }

        var records = await query.OrderBy(technician => technician.FullName)
            .Select(technician => new { Technician = technician, SpecialtyCount = technician.Specialties.Count })
            .ToListAsync(cancellationToken);
        return records.Select(record => ResponseMapper.ToTechnicianSummary(record.Technician, record.SpecialtyCount)).ToArray();
    }

    public async Task<TechnicianDetailDto> GetTechnicianAsync(int id, CancellationToken cancellationToken)
    {
        var technician = await GetTechnicianEntityAsync(id, cancellationToken);
        var specialtyCount = await _context.TechnicianSpecialties.CountAsync(
            relation => relation.TechnicianId == id, cancellationToken);
        return ResponseMapper.ToTechnicianDetail(technician, specialtyCount, DateTime.UtcNow);
    }

    public async Task<TechnicianDetailDto> CreateTechnicianAsync(
        TechnicianWriteRequest request, CancellationToken cancellationToken)
    {
        var identification = request.Identification.Trim();
        if (await _context.Technicians.AnyAsync(item => item.Identification == identification, cancellationToken))
        {
            throw new BusinessConflictException("Ya existe un técnico con esa identificación.");
        }

        var technician = new Technician();
        ApplyTechnicianRequest(technician, request);
        _context.Technicians.Add(technician);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetTechnicianAsync(technician.Id, cancellationToken);
    }

    public async Task<TechnicianDetailDto> UpdateTechnicianAsync(
        int id, TechnicianWriteRequest request, CancellationToken cancellationToken)
    {
        var technician = await _context.Technicians.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el técnico con id {id}.");
        var identification = request.Identification.Trim();
        if (await _context.Technicians.AnyAsync(
            item => item.Id != id && item.Identification == identification, cancellationToken))
        {
            throw new BusinessConflictException("Ya existe otro técnico con esa identificación.");
        }

        ApplyTechnicianRequest(technician, request);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetTechnicianAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialtySummaryDto>> GetSpecialtiesAsync(CancellationToken cancellationToken)
    {
        var records = await _context.Specialties.AsNoTracking()
            .OrderBy(specialty => specialty.Name)
            .Select(specialty => new { Specialty = specialty, TechnicianCount = specialty.Technicians.Count })
            .ToListAsync(cancellationToken);
        return records.Select(record => ResponseMapper.ToSpecialtySummary(record.Specialty, record.TechnicianCount)).ToArray();
    }

    public async Task<SpecialtyDetailDto> GetSpecialtyAsync(int id, CancellationToken cancellationToken)
    {
        var specialty = await GetSpecialtyEntityAsync(id, cancellationToken);
        var technicianCount = await _context.TechnicianSpecialties.CountAsync(
            relation => relation.SpecialtyId == id, cancellationToken);
        return ResponseMapper.ToSpecialtyDetail(specialty, technicianCount);
    }

    public async Task<SpecialtyDetailDto> CreateSpecialtyAsync(
        SpecialtyWriteRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await _context.Specialties.AnyAsync(item => item.Name == name, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe una especialidad llamada '{name}'.");
        }

        var specialty = new Specialty { Name = name, Description = request.Description?.Trim() };
        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSpecialtyAsync(specialty.Id, cancellationToken);
    }

    public async Task<SpecialtyDetailDto> UpdateSpecialtyAsync(
        int id, SpecialtyWriteRequest request, CancellationToken cancellationToken)
    {
        var specialty = await _context.Specialties.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe la especialidad con id {id}.");
        var name = request.Name.Trim();
        if (await _context.Specialties.AnyAsync(
            item => item.Id != id && item.Name == name, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe otra especialidad llamada '{name}'.");
        }

        specialty.Name = name;
        specialty.Description = request.Description?.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSpecialtyAsync(id, cancellationToken);
    }

    public async Task<TechnicianDetailDto> AddSpecialtyAsync(
        int technicianId, int specialtyId, TechnicianSpecialtyRequest request, CancellationToken cancellationToken)
    {
        var technicianExists = await _context.Technicians.AnyAsync(item => item.Id == technicianId, cancellationToken);
        if (!technicianExists)
        {
            throw new EntityNotFoundException($"No existe el técnico con id {technicianId}.");
        }

        var specialtyExists = await _context.Specialties.AnyAsync(item => item.Id == specialtyId, cancellationToken);
        if (!specialtyExists)
        {
            throw new EntityNotFoundException($"No existe la especialidad con id {specialtyId}.");
        }

        if (request.CertifiedSince.Date < new DateTime(1900, 1, 1) ||
            request.CertifiedSince.Date > DateTime.UtcNow.Date)
        {
            throw new BusinessValidationException(
                nameof(request.CertifiedSince), "La fecha de certificación debe estar entre 1900 y hoy.");
        }

        if (await _context.TechnicianSpecialties.AnyAsync(
            relation => relation.TechnicianId == technicianId && relation.SpecialtyId == specialtyId,
            cancellationToken))
        {
            throw new BusinessConflictException("El técnico ya tiene asociada esa especialidad.");
        }

        _context.TechnicianSpecialties.Add(new TechnicianSpecialty
        {
            TechnicianId = technicianId,
            SpecialtyId = specialtyId,
            CertificationLevel = request.CertificationLevel.Trim(),
            CertifiedSince = request.CertifiedSince
        });
        await _context.SaveChangesAsync(cancellationToken);
        return await GetTechnicianAsync(technicianId, cancellationToken);
    }

    public async Task RemoveSpecialtyAsync(
        int technicianId, int specialtyId, CancellationToken cancellationToken)
    {
        var relation = await _context.TechnicianSpecialties.FirstOrDefaultAsync(
            item => item.TechnicianId == technicianId && item.SpecialtyId == specialtyId,
            cancellationToken);
        if (relation is null)
        {
            throw new EntityNotFoundException("El técnico no tiene asociada esa especialidad.");
        }

        _context.TechnicianSpecialties.Remove(relation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Technician> GetTechnicianEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.Technicians.AsNoTracking()
            .Include(technician => technician.Specialties)
                .ThenInclude(relation => relation.Specialty)
            .FirstOrDefaultAsync(technician => technician.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe el técnico con id {id}.");

    private async Task<Specialty> GetSpecialtyEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.Specialties.AsNoTracking()
            .Include(specialty => specialty.Technicians)
                .ThenInclude(relation => relation.Technician)
            .FirstOrDefaultAsync(specialty => specialty.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe la especialidad con id {id}.");

    private static void ApplyTechnicianRequest(Technician technician, TechnicianWriteRequest request)
    {
        technician.FullName = request.FullName.Trim();
        technician.Identification = request.Identification.Trim();
        technician.Email = request.Email?.Trim();
        technician.Phone = request.Phone?.Trim();
        technician.IsAvailable = request.IsAvailable;
        technician.HourlyRate = request.HourlyRate;
    }
}
