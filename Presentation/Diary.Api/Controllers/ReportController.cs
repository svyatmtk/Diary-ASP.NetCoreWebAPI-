using System.Net.Mime;
using Asp.Versioning;
using Diary.Domain.Dto.ReportDto;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers
{
    /// <summary>
    /// Контроллер для управления отчётами
    /// </summary>
    [Consumes(MediaTypeNames.Application.Json)]
    //[Authorize(Roles = "User")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Выводит список всех отчётов
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <response code = "200">Если отчёты были выведены</response>
        /// <response code = "400">Если отчёты не были выведены</response>
        [HttpGet("all-reports/{userId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<ReportDto>>> GetReportsAsync(long userId)
        {
            var report = await _reportService.GetReportsAsync(userId);

            if (report.IsSuccess)
            {
                return Ok(report);
            }

            return BadRequest(report);
        }

        /// <summary>
        /// Выводит один отчёт по id
        /// </summary>
        /// <param name="id">id отчёта</param>
        /// <response code = "200">Если отчёт был выведен</response>
        /// <response code = "400">Если отчёт не был выведен</response>
        [HttpGet("one-report/{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetReportAsync(long id)
        {
            var report = await _reportService.GetReportByIdAsync(id);

            if (report.IsSuccess)
            {
                return Ok(report);
            }

            return BadRequest(report);
        }

        /// <summary>
        /// Создаёт новый отчёт
        /// </summary>
        /// <param name="dto">данные для создания отчёта</param>
        /// <remarks>
        /// Request for create report:
        /// 
        ///     POST
        ///     {
        ///         "name": "Report #1",
        ///         "description": "Test Report",
        ///         "userId": 1
        ///     }
        /// 
        /// </remarks>
        /// <response code = "200">Если отчёт был создан</response>
        /// <response code = "400">Если отчёт не был создан</response>
        [HttpPost("new-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> CreateReportAsync([FromBody] CreateReportDto dto)
        {
            var report = await _reportService.CreateReportAsync(dto);

            if (report.IsSuccess)
            {
                return Ok(report);
            }

            return BadRequest(report);
        }

        /// <summary>
        /// Обновляет существующий отчёт
        /// </summary>
        /// <param name="dto">данные для обновления отчёта</param>
        /// <remarks>
        /// Request for update report:
        /// 
        ///     PUT
        ///     {
        ///         "id": 1,
        ///         "name": "Test Report",
        ///         "description": "Test Description"
        ///     }
        /// 
        /// </remarks>
        /// <response code = "200">Если отчёт был обновлён</response>
        /// <response code = "400">Если отчёт не был обновлён</response>
        [HttpPut("update-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> UpdateReportAsync([FromBody] UpdateReportDto dto)
        {
            var report = await _reportService.UpdateReportAsync(dto);

            if (report.IsSuccess)
            {
                return Ok(report);
            }

            return BadRequest(report);
        }


        /// <summary>
        /// Удаляет отчёт
        /// </summary>
        /// <param name="dto">id отчёта</param>
        /// <param name="id"></param>
        /// <response code = "200">Если отчёт был удалён</response>
        /// <response code = "400">Если отчёт не был удалён</response>
        [HttpDelete("delete-report/{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> DeleteReportAsync(long id)
        {
            var report = await _reportService.DeleteReportAsync(id);

            if (report.IsSuccess)
            {
                return Ok(report);
            }

            return BadRequest(report);
        }
    }
}