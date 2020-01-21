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
        private readonly ITransactionRepository _transactionRepository;
        public MaterialTypeService(IAsyncRepository<MaterialType> materialTypeRepository,
                                   IAsyncRepository<MaterialDicType> materialDicTypeRepository,
                                   IAsyncRepository<MaterialDicTypeArea> materialDicTypeAreaRepository,
                                   ITransactionRepository transaction)
        {
            this._materialTypeRepository = materialTypeRepository;
            this._materialDicTypeRepository = materialDicTypeRepository;
            this._materialDicTypeAreaRepository = materialDicTypeAreaRepository;
            this._transactionRepository = transaction;
        }

        public async Task AddMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType,nameof(materialType));
            var materialTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
            var materialTypes = await this._materialTypeRepository.ListAsync(materialTypeSpec);
            if (materialTypes.Count > 0) throw new Exception(string.Format("物料类型[{0}],已经存在！", materialType.TypeName));
            await this._materialTypeRepository.AddAsync(materialType);
        }


        public async Task DelMaterialType(List<int> ids)
        {
            Guard.Against.NullOrEmpty(ids, nameof(ids));
            MaterialTypeSpecification mTypeSpec = new MaterialTypeSpecification(null, null, null);
            MaterialDicTypeSpecification materialDicTypeSpec=new MaterialDicTypeSpecification(null,null,null,null,null);
            var materialDicTypes = await this._materialDicTypeRepository.ListAsync(materialDicTypeSpec);
            var materialTypes = await this._materialTypeRepository.ListAsync(mTypeSpec);
            
            List<MaterialType> delTypes = new List<MaterialType>( );
            List<MaterialDicType> delDicTypes = new List<MaterialDicType>( );
            ids.ForEach(async (id)=>{
                delTypes.Add(materialTypes.Find(m=>m.Id==id));
                delDicTypes.AddRange(materialDicTypes.FindAll(m=>m.MaterialTypeId==id));
                await _Del(id, delTypes, delDicTypes, materialTypes, materialDicTypes);
            });
            this._transactionRepository.Transaction(async () =>
            {
                if(delTypes.Count>0)
                await this._materialTypeRepository.DeleteAsync(delTypes);
                if(delDicTypes.Count>0)
                await this._materialDicTypeRepository.DeleteAsync(delDicTypes);
            });

        }

        private async Task _Del(int cur,List<MaterialType> delTypes,
                                List<MaterialDicType> delDicTypes,
                                List<MaterialType> allMaterialTypes,
                                List<MaterialDicType> allMaterialDicTypes)
        {
            var childMaterialTypes = allMaterialTypes.FindAll(m=>m.ParentId==cur);
            childMaterialTypes.ForEach(async(c) =>
            {
                delTypes.Add(c);
                var childMaterialDicTypes = allMaterialDicTypes.FindAll(mdt => mdt.MaterialTypeId == c.Id);
                childMaterialDicTypes.ForEach(cdt => delDicTypes.Add(cdt));
                await _Del(c.Id, delTypes,delDicTypes,allMaterialTypes,allMaterialDicTypes);
            });
           
        }

        public async Task UpdateMaterialType(MaterialType materialType)
        {
            Guard.Against.Null(materialType,nameof(materialType));
            Guard.Against.NullOrEmpty(materialType.TypeName,nameof(materialType.TypeName));
            Guard.Against.Zero(materialType.Id, nameof(materialType.Id));
            MaterialTypeSpecification mTypeSpec = new MaterialTypeSpecification(null, null, materialType.TypeName);
            var types = await this._materialTypeRepository.ListAsync(mTypeSpec);
            if (types.Count > 0) throw new Exception(string.Format("物料类型名称[{0}],已经存在！", materialType.TypeName));
            mTypeSpec = new MaterialTypeSpecification(materialType.Id, null, null);
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
