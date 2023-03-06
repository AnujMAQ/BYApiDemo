using BYApiDemo.Models;
using BYApiDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<string> CreateOrUpdateColumn(List<ColumnData> requestData, bool IsNew = false)
        {
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));
            Stopwatch sp = new Stopwatch();
            sp.Start();
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
                sp.Stop();
                TimeSpan ts = sp.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

                return elapsedTime;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("Column/Delete")]
        public async Task<string> DeleteColumn(List<ColumnData> requestData)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            await _datasetsService.DeleteColumn(requestData);
            sp.Stop();
            TimeSpan ts = sp.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            return elapsedTime;
        }

        [HttpPost]
        [Route("Measure")]
        public async Task<IActionResult> CreateOrUpdateMeasure(List<MeasureData> requestData, bool IsNew = false)
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
        [Route("Measure/Delete")]
        public async Task<string> DeleteMeasure(List<MeasureData> requestData)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            await _datasetsService.DeleteMeasure(requestData);
            sp.Stop();
            TimeSpan ts = sp.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            return elapsedTime;
        }

        [HttpPost]
        [Route("Publish")]
        public async Task Publish(string repository)
        {
            if (string.IsNullOrEmpty(repository)) throw new ArgumentNullException(nameof(repository));
            await _datasetsService.PublishDataset(repository);
        }

        [HttpPost]
        [Route("UpdateAll")]

        public async Task<string> UpdateAllTables(List<ColumnData> requestData)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            await _datasetsService.UpdateAllTables(requestData);
            sp.Stop();
            TimeSpan ts = sp.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            return elapsedTime;
        }
        [HttpPost]
        [Route("DeleteTable")]
        public async Task<string> DeleteTable(List<TableData> requestData)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            await _datasetsService.DeleteTable(requestData);
            sp.Stop();
            TimeSpan ts = sp.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            return elapsedTime;
        }

    }
}
