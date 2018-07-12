using System;
using System.Linq;
using System.Collections.Generic;

namespace MVC5CourseHomework.Models
{
    public  class 客戶資料Repository : EFRepository<客戶資料>, I客戶資料Repository
    {
        public IQueryable<客戶資料> All()
        {
            return base.All().Where(w => w.是否已刪除 == false);
        }

        internal IQueryable<客戶資料> Search(string custName, string custUid, string custTel, string custFax, string custCategory)
        {
            var data = this.All();

            if (!string.IsNullOrEmpty(custName))
                data = data.Where(w => w.客戶名稱.Contains(custName));

            if (!string.IsNullOrEmpty(custUid))
                data = data.Where(w => w.統一編號.Contains(custUid));

            if (!string.IsNullOrEmpty(custTel))
                data = data.Where(w => w.電話.Contains(custTel));

            if (!string.IsNullOrEmpty(custFax))
                data = data.Where(w => w.傳真.Contains(custFax));

            if (!string.IsNullOrEmpty(custCategory))
                data = data.Where(w => w.客戶分類.Equals(custCategory));

            return data;
        }

        internal 客戶資料 Find(int id)
        {
            return this.All().FirstOrDefault(f => f.Id == id);
        }

        public override void Delete(客戶資料 entity)
        {
            entity.是否已刪除 = true;
        }

        public IQueryable<string> GetCustomerCategory()
        {
            return this.All().Select(s => s.客戶分類).Distinct();
        }

        internal IQueryable<客戶對應聯絡人及銀行帳戶數量ViewModel> GetContantBankCount(
            IQueryable<客戶聯絡人> contantData, IQueryable<客戶銀行資訊> bankData)
        {
            var data = (
                from customer in this.All()
                select new 客戶對應聯絡人及銀行帳戶數量ViewModel()
                {
                    客戶Id = customer.Id,
                    客戶名稱 = customer.客戶名稱,
                    聯絡人數量 = contantData.Where(w => w.客戶Id.Equals(customer.Id)).Count(),
                    銀行帳戶數量 = bankData.Where(w => w.客戶Id.Equals(customer.Id)).Count()
                })
                .Take(10);

            return data;
        }
    }

    public  interface I客戶資料Repository : IRepository<客戶資料>
    {

    }
}