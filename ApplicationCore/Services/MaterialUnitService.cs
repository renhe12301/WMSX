using System;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class MaterialUnitService:IMaterialUnitService
    {
        private readonly IAsyncRepository<MaterialUnit> _materialUnitRepository;
        public MaterialUnitService(IAsyncRepository<MaterialUnit> materialUnitRepository)
        {
            this._materialUnitRepository = materialUnitRepository;
        }

        public async Task AddMaterialUnit(MaterialUnit materialUnit)
        {
            Guard.Against.Null(materialUnit, nameof(materialUnit));
            Guard.Against.NullOrEmpty(materialUnit.UnitName, nameof(materialUnit.UnitName));
            await this._materialUnitRepository.AddAsync(materialUnit);
        }

        public async Task DelMaterialUnit(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            MaterialUnitSpecification materialUnitSpec = new MaterialUnitSpecification(id,null);
            var units = await this._materialUnitRepository.ListAsync(materialUnitSpec);
            Guard.Against.Zero(units.Count, nameof(units.Count));
            var unit = units[0];
            await this._materialUnitRepository.DeleteAsync(unit);

        }

        public async Task UpdateMaterialUnit(MaterialUnit materialUnit)
        {
            Guard.Against.Null(materialUnit, nameof(materialUnit));
            Guard.Against.NullOrEmpty(materialUnit.UnitName, materialUnit.UnitName);
            MaterialUnitSpecification materialUnitSpec = new MaterialUnitSpecification(materialUnit.Id, null);
            var units = await this._materialUnitRepository.ListAsync(materialUnitSpec);
            Guard.Against.Zero(units.Count, nameof(units.Count));
            var unit = units[0];
            unit.UnitName = materialUnit.UnitName;
            unit.Memo = materialUnit.Memo;
            await this._materialUnitRepository.UpdateAsync(unit);
        }
    }
}
