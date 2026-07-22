using Application.Features.Product.Dto;
using Application.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Enums;

namespace Ecommerce.Api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResult<ProductItemResponse>>> GetProducts()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> CreateProduct()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> PatchProductStock()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> PatchProductStatus()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct()
        {
            throw new NotImplementedException();
        }
    }
}
