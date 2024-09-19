﻿using DecorVista.DataAccess.Data;
using DecorVista.DataAccess.Repository.IRepository;
using DecorVista.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecorVista.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IPortfolioRepository Portfolio { get; private set; }
       
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Portfolio = new PortfolioRepository(_db);
           
        }

        public void ClearChangeTracker()
        {
            foreach (var entry in _db.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }
        }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
