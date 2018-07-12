using System;
using System.Linq;
using System.Collections.Generic;

namespace MVC5CourseHomework.Models
{
    public  class 客戶聯絡人Repository : EFRepository<客戶聯絡人>, I客戶聯絡人Repository
    {
        public IQueryable<客戶聯絡人> All()
        {
            return base.All().Where(w => w.是否已刪除 == false);
        }

        internal IQueryable<客戶聯絡人> Search(string contantName, string contantPhone, string contantTel)
        {
            var data = this.All();

            if (!string.IsNullOrEmpty(contantName))
                data = data.Where(w => w.姓名.Contains(contantName));

            if (!string.IsNullOrEmpty(contantPhone))
                data = data.Where(w => w.手機.Contains(contantPhone));

            if (!string.IsNullOrEmpty(contantTel))
                data = data.Where(w => w.電話.Contains(contantTel));

            return data;
        }

        internal 客戶聯絡人 Find(int id)
        {
            return this.All().FirstOrDefault(f => f.Id == id);
        }

        public override void Delete(客戶聯絡人 entity)
        {
            entity.是否已刪除 = true;
        }

        internal bool CheckMail(int id, string email, int 客戶id)
        {
            bool boolResult = false;

            if (id == 0)
            {
                // 新增時檢查 E-Mail 是否重複
                boolResult = this.All().Any(w => w.Email.Equals(email) && w.客戶Id.Equals(客戶id));
            }
            else
            {
                // 編輯時檢查 E-Mail 是否重複，需排除本身 id
                boolResult = this.All().Any(w => w.Email.Equals(email) && w.客戶Id.Equals(客戶id) & w.Id != id);
            }

            return boolResult;
        }
    }

    public  interface I客戶聯絡人Repository : IRepository<客戶聯絡人>
    {

    }
}