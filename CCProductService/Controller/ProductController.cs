using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Enums;
using CCApiLibrary.Models;
using CCProductService.DTOs;
using CCProductService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using JsonPatchDocument = Microsoft.AspNetCore.JsonPatch.JsonPatchDocument;

namespace CCProductService.Controller
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get a list with "<see cref="ProductBase"/>" items (using Dapper)
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ProductStandardPrice))]
        [ProducesResponseType(204)]
        [SwaggerOperation("Get a list with Product items")]
        public async Task<IActionResult> Get(int? skip, int? take)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    IEnumerable<ProductStandardPrice> productsList = null;
                    productRepository.Init(userClaim.TenantDatabase);
                    productsList = await productRepository.GetAllProducts(take, skip, userClaim).ConfigureAwait(false);
                    return Ok(productsList);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Gets a "<see cref="ProductBase"/>" by Id (using Dapper)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductBase))]
        [ProducesResponseType(404)]
        [SwaggerOperation("Gets a Product by Id")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    ProductBase product = await productRepository.GetProductById(id, userClaim).ConfigureAwait(false);
                    if (product != null)
                    {
                        return Ok(product);
                    }
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Adds a new "<see cref="ProductBase"/>" (using EF Core)
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Adds a new Product")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Post(ProductBase productDto)
        {
            try
            {
                UserClaim userClaim = null;
                Guid? newProductId = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    newProductId = await productRepository.AddProductAsync(productDto, userClaim).ConfigureAwait(false);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{newProductId}"), null);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Updates a "<see cref="Product"/>" (using EF Core)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Updates a Product")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Put(Guid id, [ModelBinder] Product productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest("The id inside the body do not match the query parameter");
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateProductAsync(productDto, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }
        }

        /// <summary>
        /// Patch a "<see cref="ProductBase"/>" using Microsoft.AspNetCore.JsonPatch. See https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 (using EF Core)
        /// </summary>
        /// <param name="id"></param>       
        /// <returns></returns>
        [HttpPatch]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Patch a Product not using Microsoft.AspNetCore.JsonPatch. See https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument productPatch)
        {
            try
            {
                ProductBase productBase;
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    productBase = await productRepository.PatchProductAsync(id, productPatch, userClaim).ConfigureAwait(false);
                    if (productBase != null)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete a "<see cref="ProductBase"/>" (using EF Core)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete a Product")]
        public async Task<IActionResult> Delete(Guid id)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);

                if (await productRepository.DeleteProductAsync(id, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }

        }

        /// <summary>
        /// Get a list of "<see cref="ProductCategory"/>" for a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        [ProducesResponseType(404)]
        [Route("{id}/categories")]
        [SwaggerOperation("Get a list of Categories for a product")]
        public async Task<IActionResult> GetProductCategories(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<ProductCategory> productCategories = await productRepository.GetCategoriesAsnyc(id,CategoryPoolType.PoolTypeCategory, userClaim).ConfigureAwait(false);
                    if (productCategories != null && productCategories.Count() > 0)
                    {
                        return Ok(productCategories);
                    }
                    return NotFound(); 
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}/barcodes")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(404)]
        [SwaggerOperation("Get a list of barcodes for the product")]
        public async Task<IActionResult> GetProductBarcodes(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<string> barcodes = await productRepository.GetBarcodesAsync(id, userClaim);
                    if (barcodes != null && barcodes.Count() > 0)
                    {
                        return Ok(barcodes);
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}/pricings")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductPrice>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [SwaggerOperation("Get a list of the current prices for the product")]
        public async Task<IActionResult> GetProductPricings(Guid id, string Datestring)
        {            
            if (DateTimeOffset.TryParse(Datestring, out DateTimeOffset currentDate))
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    var pricings = await productRepository.GetProductPrices(id, currentDate, userClaim);
                    if (pricings != null)
                    {
                        return Ok(pricings);
                    }
                    return NotFound();
                }

            }
            else
            {
                return BadRequest("The given DateString is not a valid Datetime");
            }
           
        }

        [HttpGet]
        [Route("{id}/pricings/history")]
        [SwaggerOperation("Get a history of prices for the product")]
        public async Task<IActionResult> GetPricingHistory(Guid id, string startDate, string endDate)
        {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    var pricingsHistory = await productRepository.GetPricingHistory(id, startDate, endDate, userClaim);
                    return Ok(pricingsHistory);
                }
        }

        [HttpPost]
        [Route("{id}/pricings")]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Sets prices for the product")]
        public async Task<IActionResult> SetProductPricings(Guid productId, List<ProductPriceBase> productPriceBase)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    var pricings = await productRepository.AddProductPrices(productId, productPriceBase, userClaim);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{pricings}"), null);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("{id}/pricings")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("PutMethod: Updates a ProductPrice via put request")]
        public async Task<IActionResult> Put(Guid productId, ProductPriceBase productPriceBase)
        {
            try
            {
                if (productId != productPriceBase.ProductId)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    if (await productRepository.UpdateProductPrice(productId, productPriceBase, userClaim).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("{id}/pricings")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete a ProductPrice")]
        public async Task<IActionResult> DeleteProductPrice(Guid productId)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    if (await productRepository.DeleteProductPrice(productId).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }



            //[HttpGet]
            //[Route("{id}/compilations")]
            //public async Task<IActionResult> GetProductCompilations(Guid id)
            //{
            //    return Ok();
            //}

            //[HttpGet]
            //[Route("{id}/categories")]
            //public async Task<IActionResult> GetProductPools()
            //{
            //    return Ok();
            //}

            //[HttpGet]
            //[Route("{id}/categories")]
            //public async Task<IActionResult> GetCategories(Guid id)
            //{
            //    return Ok();
            //}

            //app.MapDelete("/products/{id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (Guid id, HttpContext context) => {

            //}).WithMetadata(new SwaggerOperationAttribute("Delet a product (using EF Core)"));

            //app.MapGet("products/{id}/categories", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (Guid id, HttpContext context) => {

            //}).WithMetadata(new SwaggerOperationAttribute("Get a list of categories for this product (using Dapper)"));

            //app.MapGet("products/{id}/barcodes", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (Guid id, HttpContext context) => {

            //}).WithMetadata(new SwaggerOperationAttribute("Get a list of barcodes for this product (using Dapper)"));

            //app.MapGet("products/{id}/pricings", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (Guid id, HttpContext context) => {

            //}).WithMetadata(new SwaggerOperationAttribute("Get price definitions (using Dapper)"));

            //app.MapGet("products/{id}/compilations", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (Guid id, HttpContext context) => {

            //}).WithMetadata(new SwaggerOperationAttribute("Get product compilations (using Dapper)"));

        }

        [HttpPost]
        [Route("{id}/categories")]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Sets Category By ProductID")]
        public async Task<IActionResult> SetCategoryByProductId(Guid productId, ProductCategory productCategory)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                var temp = await productRepository.SetCategoryByProductId(productId, productCategory, userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{temp}"), null);
            }
        }

        [HttpPut]
        [Route("{id}/categories")]
        [SwaggerOperation("Updates Category By ProductID")]
        public async Task<IActionResult> UpdateCategoryByProductId(Guid id, [ModelBinder] ProductCategory productCategory)
        {
            if (id != productCategory.ProductId)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateCategoryByProductId(id, productCategory, CategoryPoolType.PoolTypeCategory, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        [ProducesResponseType(404)]
        [Route("{id}/taxes")]
        [SwaggerOperation("Get a list of taxes for a product")]
        public async Task<IActionResult> GetProductTaxes(Guid id,CategoryPoolType categoryPoolType)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<ProductCategory> productCategories = await productRepository.GetCategoriesAsnyc(id,CategoryPoolType.PoolTypeTax, userClaim).ConfigureAwait(false);
                    if (productCategories != null && productCategories.Count() > 0)
                    {
                        return Ok(productCategories);
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        [ProducesResponseType(404)]
        [Route("{id}/tags")]
        [SwaggerOperation("Get a list of tags for a product")]
        public async Task<IActionResult> GetProductTags(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<ProductCategory> productCategories = await productRepository.GetCategoriesAsnyc(id,CategoryPoolType.PoolTypeTags, userClaim).ConfigureAwait(false);
                    if (productCategories != null && productCategories.Count() > 0)
                    {
                        return Ok(productCategories);
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        [ProducesResponseType(404)]
        [Route("{id}/additives")]
        [SwaggerOperation("Get a list of additives for a product")]
        public async Task<IActionResult> GetProductAdditives(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<ProductCategory> productCategories = await productRepository.GetCategoriesAsnyc(id,CategoryPoolType.PoolTypeMenuPlan, userClaim).ConfigureAwait(false);
                    if (productCategories != null && productCategories.Count() > 0)
                    {
                        return Ok(productCategories);
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductCategory>))]
        [ProducesResponseType(404)]
        [Route("{id}/allergens")]
        [SwaggerOperation("Get a list of allergens for a product")]
        public async Task<IActionResult> GetProductAllergens(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
                {
                    productRepository.Init(userClaim.TenantDatabase);
                    IEnumerable<ProductCategory> productCategories = await productRepository.GetCategoriesAsnyc(id,CategoryPoolType.PoolTypeMenuPlan, userClaim).ConfigureAwait(false);
                    if (productCategories != null && productCategories.Count() > 0)
                    {
                        return Ok(productCategories);
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //[HttpPost]
        //[Route("{id}/taxes")]
        //[ProducesResponseType(201)]
        //[ProducesResponseType(422)]
        //[ServiceFilter(typeof(ValidateModelAttribute))]
        //[SwaggerOperation("Sets Taxes By ProductID")]
        //public async Task<IActionResult> SetTaxesByProductId(Guid productId, ProductCategory productCategory)
        //{
        //    UserClaim userClaim = null;
        //    if (HttpContext.User.Claims != null)
        //    {
        //        userClaim = new UserClaim(HttpContext.User.Claims);
        //    }

        //    using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
        //    {
        //        productRepository.Init(userClaim.TenantDatabase);
        //        var temp = await productRepository.SetCategoryByProductId(productId, productCategory,CategoryPoolType.PoolTypeTax, userClaim);
        //        return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{temp}"), null);
        //    }
        //}

        //[HttpPost]
        //[Route("{id}/tags")]
        //[ProducesResponseType(201)]
        //[ProducesResponseType(422)]
        //[ServiceFilter(typeof(ValidateModelAttribute))]
        //[SwaggerOperation("Sets Tags By ProductID")]
        //public async Task<IActionResult> SetTagsByProductId(Guid productId, ProductCategory productCategory)
        //{
        //    UserClaim userClaim = null;
        //    if (HttpContext.User.Claims != null)
        //    {
        //        userClaim = new UserClaim(HttpContext.User.Claims);
        //    }

        //    using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
        //    {
        //        productRepository.Init(userClaim.TenantDatabase);
        //        var temp = await productRepository.SetCategoryByProductId(productId, productCategory,CategoryPoolType.PoolTypeTags, userClaim);
        //        return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{temp}"), null);
        //    }
        //}

        //[HttpPost]
        //[Route("{id}/additives")]
        //[ProducesResponseType(201)]
        //[ProducesResponseType(422)]
        //[ServiceFilter(typeof(ValidateModelAttribute))]
        //[SwaggerOperation("Sets Additives By ProductID")]
        //public async Task<IActionResult> SetAdditivesByProductId(Guid productId, ProductCategory productCategory)
        //{
        //    UserClaim userClaim = null;
        //    if (HttpContext.User.Claims != null)
        //    {
        //        userClaim = new UserClaim(HttpContext.User.Claims);
        //    }

        //    using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
        //    {
        //        productRepository.Init(userClaim.TenantDatabase);
        //        var temp = await productRepository.SetCategoryByProductId(productId, productCategory, CategoryPoolType.PoolTypeMenuPlan, userClaim);
        //        return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{temp}"), null);
        //    }
        //}

        //[HttpPost]
        //[Route("{id}/allergens")]
        //[ProducesResponseType(201)]
        //[ProducesResponseType(422)]
        //[ServiceFilter(typeof(ValidateModelAttribute))]
        //[SwaggerOperation("Sets Allergens By ProductID")]
        //public async Task<IActionResult> SetAllergensByProductId(Guid productId, ProductCategory productCategory)
        //{
        //    UserClaim userClaim = null;
        //    if (HttpContext.User.Claims != null)
        //    {
        //        userClaim = new UserClaim(HttpContext.User.Claims);
        //    }

        //    using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
        //    {
        //        productRepository.Init(userClaim.TenantDatabase);
        //        var temp = await productRepository.SetCategoryByProductId(productId, productCategory, CategoryPoolType.PoolTypeMenuPlan, userClaim);
        //        return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{temp}"), null);
        //    }
        //}

        [HttpPut]
        [Route("{id}/tags")]
        [SwaggerOperation("Updates Tags By ProductID")]
        public async Task<IActionResult> UpdateTagsByProductId(Guid id, [ModelBinder] ProductCategory productCategory)
        {
            if (id != productCategory.ProductId)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateCategoryByProductId(id, productCategory, CategoryPoolType.PoolTypeTags, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }

        }

        [HttpPut]
        [Route("{id}/taxes")]
        [SwaggerOperation("Updates Taxes By ProductID")]
        public async Task<IActionResult> UpdateTaxesByProductId(Guid id, [ModelBinder] ProductCategory productCategory)
        {
            if (id != productCategory.ProductId)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateCategoryByProductId(id, productCategory, CategoryPoolType.PoolTypeTax, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }

        }

        [HttpPut]
        [Route("{id}/additives")]
        [SwaggerOperation("Updates Additives By ProductID")]
        public async Task<IActionResult> UpdateAdditivesByProductId(Guid id, [ModelBinder] ProductCategory productCategory)
        {
            if (id != productCategory.ProductId)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateCategoryByProductId(id, productCategory, CategoryPoolType.PoolTypeMenuPlan, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }

        }

        [HttpPut]
        [Route("{id}/allergens")]
        [SwaggerOperation("Updates Allergens By ProductID")]
        public async Task<IActionResult> UpdateAllergensByProductId(Guid id, [ModelBinder] ProductCategory productCategory)
        {
            if (id != productCategory.ProductId)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductRepository productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                productRepository.Init(userClaim.TenantDatabase);
                if (await productRepository.UpdateCategoryByProductId(id, productCategory, CategoryPoolType.PoolTypeMenuPlan, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }
        }
    }
}
