﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using MVC5CourseHomework.Models;

namespace MVC5CourseHomework.Controllers
{
    public class 客戶資料Controller : Controller
    {
        客戶資料Repository customerRepo;
        View_客戶對應聯絡人及銀行帳戶數量Repository dbViewRepo;
        客戶聯絡人Repository contantRepo;
        客戶銀行資訊Repository bankRepo;

        public 客戶資料Controller()
        {
            customerRepo = RepositoryHelper.Get客戶資料Repository();
            dbViewRepo = RepositoryHelper.GetView_客戶對應聯絡人及銀行帳戶數量Repository(customerRepo.UnitOfWork);
            contantRepo = RepositoryHelper.Get客戶聯絡人Repository(customerRepo.UnitOfWork);
            bankRepo = RepositoryHelper.Get客戶銀行資訊Repository(customerRepo.UnitOfWork);
        }

        // GET: 客戶資料
        public ActionResult Index()
        {
            var data = customerRepo.All();

            var categoryData = customerRepo.GetCustomerCategory();
            ViewBag.custCategory = new SelectList(categoryData);

            return View(data);
        }

        public ActionResult Search(string custName, string custUid, string custTel, string custFax, string custCategory)
        {
            var data = customerRepo.Search(custName, custUid, custTel, custFax, custCategory);
            var categoryData = customerRepo.GetCustomerCategory();
            ViewBag.custCategory = new SelectList(categoryData);
            return View("Index", data);
        }

        // GET: 客戶資料/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶資料 客戶資料 = customerRepo.Find(id.Value);
            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // GET: 客戶資料/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: 客戶資料/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,客戶名稱,統一編號,電話,傳真,地址,Email,客戶分類")] 客戶資料 客戶資料)
        {
            if (ModelState.IsValid)
            {
                customerRepo.Add(客戶資料);
                customerRepo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }

            return View(客戶資料);
        }

        // GET: 客戶資料/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶資料 客戶資料 = customerRepo.Find(id.Value);
            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // POST: 客戶資料/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,客戶名稱,統一編號,電話,傳真,地址,Email,客戶分類")] 客戶資料 客戶資料)
        {
            if (ModelState.IsValid)
            {
                var db = customerRepo.UnitOfWork.Context;
                db.Entry(客戶資料).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(客戶資料);
        }

        // GET: 客戶資料/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶資料 客戶資料 = customerRepo.Find(id.Value);
            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // POST: 客戶資料/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            客戶資料 客戶資料 = customerRepo.Find(id);
            customerRepo.Delete(客戶資料);
            customerRepo.UnitOfWork.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult DbViewList()
        {
            var data = dbViewRepo.All();
            return View(data);
        }

        public ActionResult ViewModelList()
        {
            var contantData = contantRepo.All();
            var bankData = bankRepo.All();
            var data = customerRepo.GetContantBankCount(contantData, bankData);
            return View(data);
        }

        public ActionResult Export(string custName, string custUid, string custTel, string custFax, string custCategory)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                int timeStamp = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

                var data = customerRepo
                    .Search(custName, custUid, custTel, custFax, custCategory)
                    .Select(s => new { s.Id, s.客戶名稱, s.統一編號, s.電話, s.傳真, s.地址, s.Email, s.客戶分類 });

                var ws = wb.Worksheets.Add("cusdata", 1);

                ws.Cell("A1").Value = "Id";
                ws.Cell("B1").Value = "客戶名稱";
                ws.Cell("C1").Value = "統一編號";
                ws.Cell("D1").Value = "電話";
                ws.Cell("E1").Value = "傳真";
                ws.Cell("F1").Value = "地址";
                ws.Cell("G1").Value = "Email";
                ws.Cell("H1").Value = "客戶分類";

                ws.Cell(2, 1).InsertData(data);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return File(
                        memoryStream.ToArray(),
                        "application/vnd.ms-excel",
                        $"Export_客戶資料_{timeStamp}.xlsx");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                customerRepo.UnitOfWork.Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
