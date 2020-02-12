using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
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

        public async Task AddMaterialType(MaterialType materialType,bool unique=false)
        {
            Guard.Against.Null(materialType, nameof(materialType));
            if (unique)
            {
                var materialTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
                var materialTypes = await this._materialTypeRepository.ListAsync(materialTypeSpec);
                if (materialTypes.Count == 0)
                    await this._materialTypeRepository.AddAsync(materialType);
            }
            else
            {
                await this._materialTypeRepository.AddAsync(materialType);
            }
        }

        public async Task AddMaterialType(List<MaterialType> materialTypes,bool unique=false)
        {
            Guard.Against.Null(materialTypes, nameof(materialTypes));
            if (unique)
            {
                List<MaterialType> adds = new List<MaterialType>();
                materialTypes.ForEach(async (mt) =>
                {
                    var materialTypeSpec = new MaterialTypeSpecification(null, null, mt.TypeName);
                    var mts = await this._materialTypeRepository.ListAsync(materialTypeSpec);
                    if(mts.Count==0)
                        adds.Add(mts.First());
                });
                if (adds.Count > 0)
                    await this._materialTypeRepository.AddAsync(adds);
            }
            else
            {
                await this._materialTypeRepository.AddAsync(materialTypes);
            }
            
        }

        public async Task UpdateMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType, nameof(materialType));
            await this._materialTypeRepository.UpdateAsync(materialType);
        }

        public async Task UpdateMaterialType(List<MaterialType> materialTypes)
        {
            Guard.Against.Null(materialTypes, nameof(materialTypes));
            await this._materialTypeRepository.UpdateAsync(materialTypes);
        }
        
    }
}
