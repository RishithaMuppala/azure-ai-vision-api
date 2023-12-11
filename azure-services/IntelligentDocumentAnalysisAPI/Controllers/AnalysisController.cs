using IntelligentDocumentAnalysisAPI.DTOs;
using IntelligentDocumentAnalysisAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentDocumentAnalysisAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalyzeImageService _analyzeImageService;
        private readonly ExtractHandWrittenTextService _readHandWrittenTextService;
        private readonly ExtractPrintedText _extractPrintedText;

        public AnalysisController(AnalyzeImageService analyzeImageService, ExtractHandWrittenTextService readHandWrittenTextService, ExtractPrintedText extractPrintedText) {
            _analyzeImageService = analyzeImageService;
            _readHandWrittenTextService = readHandWrittenTextService;
            _extractPrintedText = extractPrintedText;
        }

        [HttpPost("read/handwritten")]
        public async Task<IActionResult> ExtractHandWrittenText([FromForm] AnalysisDTO model)
        {
            try
            {
                var result = await _readHandWrittenTextService.ReadText(model.Image);
                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("read/printedText")]
        public async Task<IActionResult> ExtractPrintedText([FromForm] AnalysisDTO model)
        {
            try
            {
                var result = await _extractPrintedText.MakeOCRRequest(model.Image);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("image")]
        public async Task<IActionResult> Image([FromForm] AnalysisDTO model)
        {
            try
            {
                var result = await _analyzeImageService.MakeAnalysisRequest(model.Image);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
