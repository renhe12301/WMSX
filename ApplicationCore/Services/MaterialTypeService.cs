using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Security;
using System.Transactions;

namespace ApplicationCore.Services
{
    public class MaterialTypeService : IMaterialTypeService
    {

        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;

        public MaterialTypeService(IAsyncRepository<MaterialType> materialTypeRepository)
        {
            this._materialTypeRepository = materialTypeRepository;
        }

        public async Task AddMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType, nameof(materialType));
            var materialTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
            var materialTypes = await this._materialTypeRepository.ListAsync(materialTypeSpec);
            if (materialTypes.Count > 0)
                await this._materialTypeRepository.AddAsync(materialType);
        }

    }
}
