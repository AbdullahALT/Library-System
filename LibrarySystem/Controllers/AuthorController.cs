﻿using LibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibrarySystem.Controllers
{
    public class AuthorController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var authors = context.Authors.Include("Books").ToList();
                return View(authors);
            }
        }

        public ActionResult Details(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var author = context.Authors.Include("Books").ToList().Where(b => b.AuthorId == id).SingleOrDefault(); ;
                return (author != null) ? View(author) : View("Error");
            }

        }

        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            using (var context = new LibraryDatabaseContainer())
            {
                
                return View();
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create(Author author)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                context.Authors.Add(author);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int id)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                var author = context.Authors.Find(id);
                
                return (author != null)? View(author) : View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Author author)
        {
            using (var context = new LibraryDatabaseContainer())
            {
                Author oldAuthor = context.Authors.Find(author.AuthorId);
                oldAuthor.Name = author.Name;
                oldAuthor.BirthDate = author.BirthDate;
                oldAuthor.Specialty = author.Specialty;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}