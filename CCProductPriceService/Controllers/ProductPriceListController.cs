using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CCProductPriceService.Controllers
{
    public class ProductPriceListController
    {
        private IServiceProvider _serviceProvider;

        public ProductPriceListController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{

        //}

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{

        //}

        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //public async Task<IActionResult> Post()
        //{

        //}

        //[HttpPatch]
        //[Route("{id}")]
        //public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        //{

        //}

        //[HttpDelete]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[Route("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{

        //}
    }
}
