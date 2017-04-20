﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibrarySystem.Models;
using System.IO;

namespace LibrarySystem.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var books = context.Books.Include("Author").ToList();
                return View(books);
            }
        }

        public ActionResult Details(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var book = context.Books.Include("Author").ToList().Where(b => b.BookId == id).SingleOrDefault();
                return (book != null) ? View(book) : View("Error");
            }

        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var book = context.Books.Include("Author").ToList().Where(b => b.BookId == id).SingleOrDefault();
                return (book != null) ? View(book) : View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult PostDelete(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var entity = context.Books.Find(id);
                context.Books.Remove(entity);
                context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var writer = context.Authors.ToList();
                ViewData["Writer"] = new SelectList(writer, "AuthorId", "Name");
                return View();
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create(Book book, HttpPostedFileBase file)
        {
            using (var context = new LibraryDatabaseContainer())
            {

                if (ModelState.IsValid && PhotoSave(book, file))
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                    return RedirectToAction("Index"); 
                }
                var writer = context.Authors.ToList();
                ViewData["Writer"] = new SelectList(writer, "AuthorId", "Name");
                return View("Create");
            }

        }

        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var book = context.Books.Find(id);
                if (book != null)
                {
                    var writer = context.Authors.ToList();
                    ViewData["Writer"] = new SelectList(writer, "AuthorId", "Name");

                    return View(book);
                }
                return View("Error");

            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, HttpPostedFileBase file)
        {

            using (var context = new LibraryDatabaseContainer())
            {
                Book oldBook = context.Books.Find(book.BookId);
                if (ModelState.IsValid)
                {
                    if (PhotoSave(oldBook, file))
                    {
                        oldBook.Title = book.Title;
                        oldBook.Type = book.Type;
                        oldBook.Edition = book.Edition;
                        oldBook.ReleaseDate = book.ReleaseDate;
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                var writer = context.Authors.ToList();
                ViewData["Writer"] = new SelectList(writer, "AuthorId", "Name");
                return View(oldBook);
            }


        }

        private bool PhotoSave(Book book, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (file.ContentType == "image/jpg" || file.ContentType == "image/png" || file.ContentType == "image/jpeg")
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/Books"), fileName);
                    file.SaveAs(path);
                    book.Image = "~/Images/Books/" + fileName;
                    return true;
                }
                return false;
            }
            return false;
        }

    }
}