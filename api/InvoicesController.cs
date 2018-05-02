﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.DataModels;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.api
{
    [Produces("application/json")]
    [Route("api/Invoices")]
    public class InvoicesController : Controller
    {
        private readonly MyDatabaseContext _context;

        public InvoicesController(MyDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        /// <summary>
        /// GET  all Invoices.
        /// </summary>
         [HttpGet]
        public IEnumerable<Invoice> GetInvoice()
        {
            return _context.Invoice;
        }

        // GET: api/Invoices/5
        /// <summary>
        /// GET  a specific Invoice.
        /// <param name="id"></param>      
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _context.Invoice.SingleOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice([FromRoute] int id, [FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoice.InvoiceId)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invoices
        /// <summary>
        /// Post  a specific Invoice.
        /// </summary>
        /// <param name="invoice"></param>      
        /// <summary>
        /// Creates a Invoice item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 12,
        ///        "CustomerId": "1",
        ///        "InvoiceDate": "yyyy:mm:dd",
        ///        "Total" : 12
        ///     }
        ///
        /// </remarks>
        /// <param name="invoice"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(typeof(Invoice), 201)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> PostInvoice([FromBody] Invoice invoice)
        {
            if (invoice == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tempInvoice = await _context.Invoice.SingleOrDefaultAsync(m => m.InvoiceId == invoice.InvoiceId);

            if (invoice != null)
            {
                _context.Invoice.Update(invoice);
                await _context.SaveChangesAsync();
                return Ok(invoice);
            }
            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.InvoiceId }, invoice);
        }

        /// <summary>
        /// Deletes a specific Invoice.
        /// </summary>
        /// <param name="id"></param>      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _context.Invoice.SingleOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoice.Any(e => e.InvoiceId == id);
        }
    }
}