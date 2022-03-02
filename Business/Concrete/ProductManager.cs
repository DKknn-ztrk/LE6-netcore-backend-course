using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Exception;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductDal _productDal;
        private ICategoryService _categoryService; 
        // başka bir servis kullanmak istersek 
        // dal değil service i çağırmak..!

        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;

        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        //[ExceptionLogAspect(typeof(FileLogger))]
        [PerformanceAspect(5)]//5  saniyeyi geçerse bu işlem o zaman output a yazıcak
        public IDataResult<List<Product>> GetList()
        {
            Thread.Sleep(5000);// 5 saniye bekle - output vermesi için yazdık
            return new SuccessDataResult<List<Product>>(_productDal.GetList().ToList());
        }

        //[SecuredOperation("Product.List,Admin")]
        //[ExceptionLogAspect(typeof(DatabaseLogger))]
        //[LogAspect(typeof(DatabaseLogger))]
        [LogAspect(typeof(FileLogger))]
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

            // START bir ürün ismi sistemde mevcut ise bir daha girilemiyecek START     bunu buraya değilde CheckIfProductNameExists adında IResult oluşturuyoruz

            //if (_productDal.Get(p=>p.ProductName==product.ProductName)!=null)
            //{
            //    return new ErrorResult(Messages.ProductNameAlreadyExists);
            //}

            // END bir ürün ismi sistemde mevcut ise bir daha girilemiyecek END

            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName)); // burada kuralları çekiyoruz CheckIfProductNameExists, CheckIfCategoryIsEnabled category uydurma bi kural yaptık biz kendimize göre ayarlıyabiliriz.
            if (result != null)
            {
                return result;
            }

            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }

        private IResult CheckIfProductNameExists(string productName)
        {

            var result = _productDal.GetList(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }

            return new SuccessResult();
        }

        private IResult CheckIfCategoryIsEnabled()
        {
            var result = _categoryService.GetList();
            if (result.Data.Count < 10) // uydurma biz istediğimize göre uyarlıyabiliriz.
            {
                return new ErrorResult(Messages.CheckIfCategoryIsEnabled);
            }

            return new SuccessResult();
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
