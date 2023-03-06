using BYApiDemo.Models;
using BYApiDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BYApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        private readonly IDatasetService _datasetsService;

        public DatasetController(IDatasetService datasetsService)
        {
            _datasetsService = datasetsService;
        }

        [HttpPost]
        [Route("Column")]
        public async Task<IActionResult> CreateOrUpdateColumn(ColumnData requestData, bool IsNew = false)
        {
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));
            try
            {
                if (IsNew)
                {
                    await _datasetsService.CreateColumn(requestData);
                }
                else
                {
                    await _datasetsService.UpdateColumn(requestData);
                }
                return Ok();
            }
            catch (Exception e) {
                throw e;
            }
        }

        [HttpPost]
        [Route("Column/Delete")]
        public async Task DeleteColumn(ColumnData requestData)
        {
            await _datasetsService.DeleteColumn(requestData);
        }

        [HttpPost]
        [Route("Measure")]
        public async Task<IActionResult> CreateOrUpdateMeasure(MeasureData requestData, bool IsNew = false)
        {
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));
            try
            {
                if (IsNew)
                {
                    await _datasetsService.CreateMeasure(requestData);
                }
                else
                {
                    await _datasetsService.UpdateMeasure(requestData);
                }
                return Ok();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("Publish")]
        public async Task Publish(string repository)
        {
            if (string.IsNullOrEmpty(repository)) throw new ArgumentNullException(nameof(repository));
            await _datasetsService.PublishDataset(repository);
        }
    }
}
