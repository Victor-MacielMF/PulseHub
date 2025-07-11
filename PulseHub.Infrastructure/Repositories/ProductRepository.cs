﻿using Microsoft.EntityFrameworkCore;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PulseHub.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PulseHubDbContext _context;

        public ProductRepository(PulseHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(bool? isActive)
        {
            var query = _context.Products.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }


        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            product.IsActive = false;
            _context.Products.Update(product);
        }
    }
}
