using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetList()
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetList().ToList());
        }

        [CacheAspect(10)]//dakika
        public IDataResult<List<Product>> GetListByCategory(int categoryId)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetList(p => p.CategoryId == categoryId).ToList());
        }

        [ValidationAspect(typeof(ProductValidator), Priority = 1)] // STEP3: STEP 2 DEN KURTULMAK İÇİN YAZILDI Business > Validation Rules > FluentValidation'da yazılıp çekme işlemi 
        [CacheRemoveAspect("IProductService.Get")] // içersinde iproductservice.get olan cache keylerini sil -> Get, GetById, GetList, GetListByCategory
        //[CacheRemoveAspect("ICategoryService.Get")] // içersinde icategoryservice.get olan cache keylerini sil -> Get, GetById, GetList, GetListByCategory
        public IResult Add(Product product)
        {
            // Business codes
            // Business codes -> iş kuralları kodları buraya yazılır / if  bir ürünün adının 3
            // karakterten fazla veya az olmasını istemiyorsak eğer gibi gibi

            ////////// BU KODU FluentValidation'da Hazırlayıp çekiyoruz ///////////// STEP1: PRODUCT VALIDATION
            //ProductValidator productValidator = new ProductValidator();
            //var result = productValidator.Validate(product);
            //if (!result.IsValid) // Validation okay değil ise
            //{
            //    throw new ValidationException(result.Errors); // Hata, Error Exception ı gönder
            //}

            /////////// ÇEKME KODU///////// STEP2: STEP 1 DEN KURTULMAK İÇİN YAZILDI Business > Validation Rules > FluentValidation'da yazılıp çekme işlemi 
            //ValidationTool.Validate(new ProductValidator(), product);



            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }

        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductDeleted);

        }

        public IResult Update(Product product)
        {
            

            _productDal.Update(product);
            return new SuccessResult(Messages.ProductUpdated);
        }

        [TransactionScopeAspect]
        public IResult TransactionalOperation(Product product)
        {
            _productDal.Update(product);
            //_productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
    }
}
