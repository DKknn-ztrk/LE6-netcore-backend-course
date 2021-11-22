using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator:AbstractValidator<Product>
    {
        public ProductValidator()
        {
            // Length(2,30); minimum 2 karakter max 30 karakter
            // NotEmpty(); boş geçilemez, 
            //.NotEmpty().WithMessage("test"); // Magic string kullanılabilir istediğin bir mesaj için
            //.NotEmpty().WithMessage(Messages.ProductNameNotEmpty); // bunu araştır ama genel olarak standart boş bırakılıyormuş
            RuleFor(p => p.ProductName).NotEmpty();
            RuleFor(p => p.ProductName).Length(2, 30);
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(1);// Unitprice 1 e eşit veya 1 den yüksek olmalı
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryId == 1);// örnek, içeçek kategorisiId si:1 -> içecek kategorisinde bir ürünün minimum fiyatı 10 dur -> When şart koşulu burada
            //RuleFor(p => p.UnitPrice).InclusiveBetween(5, 25);// 5 ile 25 sayısı arasında
            //RuleFor(p => p.UnitPrice).ExclusiveBetween(5, 25);// 5 ile 25 sayısı dışında
            RuleFor(p => p.ProductName).Must(StartWithWithA); // örn. Kullanıcı girişi başına 2 tane 0 koymalısın so biz kendi custom kuralımızı yazabiliriz 
        }

        private bool StartWithWithA(string arg) // // örn. Kullanıcı girişi başına 2 tane 0 koymalısın so biz kendi custom kuralımızı yazabiliriz 
        {
            return arg.StartsWith("A");// Büyük A ile başlamalı diyebiliyoruz -> burda bunun gibi istediğimiz kuralı yazabiliriz.
        }
    }
}
