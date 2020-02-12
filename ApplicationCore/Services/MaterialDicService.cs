using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using System.Transactions;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class MaterialDicService:IMaterialDicService
    {
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;

        public MaterialDicService(IAsyncRepository<MaterialDic> materialDicRepository)
        {
            this._materialDicRepository = materialDicRepository;
        }

        public async Task AddMaterialDic(MaterialDic materialDic,bool unique=false)
        {
            Guard.Against.Null(materialDic, nameof(materialDic));
            Guard.Against.Zero(materialDic.Id,nameof(materialDic.Id));
            Guard.Against.NullOrEmpty(materialDic.MaterialName,
                nameof(materialDic.MaterialName));
            if (unique)
            {
                MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(materialDic.Id, null,
                    null,null, null);
                var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                if (materialDics.Count == 0)
                    await this._materialDicRepository.AddAsync(materialDic);
            }
            else
            {
                await this._materialDicRepository.AddAsync(materialDic);
            }
        }

        public async Task UpdateMaterialDic(MaterialDic materialDic)
        {
            Guard.Against.Null(materialDic, nameof(materialDic));
            await this._materialDicRepository.UpdateAsync(materialDic);
        }
        
        public async Task AddMaterialDic(List<MaterialDic> materialDics,bool unique=false)
        {
            Guard.Against.Null(materialDics, nameof(materialDics));
            if (unique)
            {
                List<MaterialDic> adds=new List<MaterialDic>();
                materialDics.ForEach(async (m) =>
                {
                    
                    MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(m.Id, null,
                        null,null, null);
                    var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
                    if (materialDics.Count == 0)
                        adds.Add(materialDics.First());
                });
                if (adds.Count > 0)
                    await this._materialDicRepository.AddAsync(adds);
            }
            else
            {
                await this._materialDicRepository.AddAsync(materialDics);
            }
        }
        
        public async Task UpdateMaterialDic(List<MaterialDic> materialDics)
        {
            Guard.Against.Null(materialDics, nameof(materialDics));
            Guard.Against.NullOrEmpty(materialDics,nameof(materialDics));
            await this._materialDicRepository.UpdateAsync(materialDics);
        }
    }
}
