using Domain.Enums;
using Application.DTOs.Category;
using Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>List of categories.</returns>
        [HttpGet]
        public Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets products by category.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns>List of products.</returns>
        [HttpGet("{id}/products")]
        public Task<ActionResult<IEnumerable<ProductDto>>> GetProductsWithinCategory(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a category.
        /// </summary>
        /// <remarks>
        /// Requires admin authorization.
        /// </remarks>
        /// <param name="dto">Category data.</param>
        /// <returns>Created category.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public Task<IActionResult> CreateCategory([FromForm] CreateCategory dto)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <remarks>
        /// Requires admin authorization.
        /// </remarks>
        /// <param name="id">Category ID.</param>
        /// <param name="dto">Updated category data.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPut("{id}")]
        public Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategory dto)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <remarks>
        /// Requires admin authorization.
        /// </remarks>
        /// <param name="id">Category ID.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }
    }
}
