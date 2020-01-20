using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;
using System.Collections.Generic;

namespace ApplicationCore.Services
{
    public class MaterialTypeService:IMaterialTypeService
    {

        private readonly IAsyncRepository<MaterialType> _materialTypeRepository;
        private readonly IAsyncRepository<MaterialDicType> _materialDicTypeRepository;
        private readonly IAsyncRepository<MaterialDicTypeArea> _materialDicTypeAreaRepository;

        public MaterialTypeService(IAsyncRepository<MaterialType> materialTypeRepository,
                                   IAsyncRepository<MaterialDicType> materialDicTypeRepository,
                                   IAsyncRepository<MaterialDicTypeArea> materialDicTypeAreaRepository)
        {
            this._materialTypeRepository = materialTypeRepository;
            this._materialDicTypeRepository = materialDicTypeRepository;
            this._materialDicTypeAreaRepository = materialDicTypeAreaRepository;
        }

        public async Task AddMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType,nameof(materialType));
            var materialTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
            var materialTypes = await this._materialTypeRepository.ListAsync(materialTypeSpec);
            if (materialTypes.Count > 0) throw new Exception(string.Format("物料类型[{0}],已经存在！", materialType.TypeName));
            await this._materialTypeRepository.AddAsync(materialType);
        }


        public async Task DelMaterialType(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            MaterialTypeSpecification mTypeSpec = new MaterialTypeSpecification(id, null, null);
            var materialTypes = await this._materialTypeRepository.ListAsync(mTypeSpec);
            Guard.Against.Zero(materialTypes.Count, nameof(materialTypes.Count));
            List<int> delIds = new List<int> { id };
            await this._Del(id,delIds);
            this._materialDicTypeRepository.TransactionScope(() =>
            {
                delIds.ForEach(async(delId) =>
                {
                    MaterialTypeSpecification delTypeSpec = new MaterialTypeSpecification(delId, null, null);
                    var delMaterialTypes = await this._materialTypeRepository.ListAsync(delTypeSpec);
                    if (delMaterialTypes.Count > 0)
                    {
                        await this._materialTypeRepository.DeleteAsync(delMaterialTypes[0]);
                        MaterialDicTypeSpecification materialDicTypeSpec = new MaterialDicTypeSpecification(null,
                                                                           id, null, null, null);
                        var dicTypes = await this._materialDicTypeRepository.ListAsync(materialDicTypeSpec);
                        await this._materialDicTypeRepository.DeleteAsync(dicTypes);

                        MaterialDicTypeAreaSpecification materialDicTypeAreaSpec = new MaterialDicTypeAreaSpecification(id, null, null);
                        var dicAreaTypes = await this._materialDicTypeAreaRepository.ListAsync(materialDicTypeAreaSpec);
                        await this._materialDicTypeAreaRepository.DeleteAsync(dicAreaTypes);
                    }
                });
            });
        }

        private async Task _Del(int cur,List<int> delIds)
        {
            MaterialTypeSpecification mTypeSpec = new MaterialTypeSpecification(null, cur, null);
            var materialTypes = await this._materialTypeRepository.ListAsync(mTypeSpec);
            materialTypes.ForEach(async(c) =>
            {
                await _Del(c.Id, delIds);
            });
           
        }

        public async Task UpdateMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType,nameof(materialType));
            Guard.Against.NullOrEmpty(materialType.TypeName,nameof(materialType.TypeName));
            Guard.Against.Zero(materialType.Id, nameof(materialType.Id));
            MaterialTypeSpecification mTypeSpec = new MaterialTypeSpecification(materialType.Id, null, null);
            var materialTypes = await this._materialTypeRepository.ListAsync(mTypeSpec);
            Guard.Against.Zero(materialTypes.Count, nameof(materialTypes.Count));
            var updMaterialType = materialTypes[0];
            if (materialType.ParentId > 0)
                updMaterialType.ParentId = materialType.ParentId;
            updMaterialType.TypeName = materialType.TypeName;
            await this._materialTypeRepository.UpdateAsync(updMaterialType);
        }

        public async Task AssignMaterialDic(int typeId, List<int> materialDicIds)
        {
            Guard.Against.Zero(typeId, nameof(typeId));
            MaterialDicTypeSpecification materialDicTypeSpec=new MaterialDicTypeSpecification(null,typeId,null,
                                                                                 null,null);
            List<MaterialDicType> mdTypes = await this._materialDicTypeRepository.ListAsync(materialDicTypeSpec);
            if (mdTypes.Count > 0)
                await this._materialDicTypeRepository.DeleteAsync(mdTypes);
            
            List<MaterialDicType> materialDicTypes=new List<MaterialDicType>();
            materialDicIds.ForEach(async (mId) =>
            {
                MaterialDicType materialDicType = new MaterialDicType
                {
                    MaterialDicId = mId,
                    MaterialTypeId=typeId
                };
                materialDicTypes.Add(materialDicType);
            });
            
            await this._materialDicTypeRepository.AddAsync(materialDicTypes);
        }
    }
}
