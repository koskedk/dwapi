using Dwapi.ExtractsManagement.Core.Interfaces.Repository.Dwh;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dwapi.Controller.ExtractDetails
{
    [Produces("application/json")]
    [Route("api/PatientAdverseEvent")]
    public class PatientAdverseEventController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ITempPatientAdverseEventExtractRepository _tempPatientAdverseEventExtractRepository;
        private readonly IPatientAdverseEventExtractRepository _patientAdverseEventExtractRepository;
        private readonly ITempPatientAdverseEventExtractErrorSummaryRepository _errorSummaryRepository;

        public PatientAdverseEventController(ITempPatientAdverseEventExtractRepository tempPatientAdverseEventExtractRepository, IPatientAdverseEventExtractRepository patientAdverseEventExtractRepository, ITempPatientAdverseEventExtractErrorSummaryRepository errorSummaryRepository)
        {
            _tempPatientAdverseEventExtractRepository = tempPatientAdverseEventExtractRepository;
            _patientAdverseEventExtractRepository = patientAdverseEventExtractRepository;
            _errorSummaryRepository = errorSummaryRepository;
        }
        [HttpGet("ValidCount")]
        public async Task<IActionResult> GetValidCount()
        {
            try
            {
                var count = await _patientAdverseEventExtractRepository.GetCount();
                return Ok(count);
            }
            catch (Exception e)
            {
                var msg = $"Error loading valid Patient Extracts";
                Log.Error(msg);
                Log.Error($"{e}");
                return StatusCode(500, msg);
            }
        }

        [HttpGet("LoadValid/{page}/{pageSize}")]
        public async Task<IActionResult> LoadValid(int? page,int pageSize)
        {
            try
            {
                var tempPatientAdverseEventExtracts = await _patientAdverseEventExtractRepository.GetAll(page,pageSize);
                return Ok(tempPatientAdverseEventExtracts.ToList());
            }
            catch (Exception e)
            {
                var msg = $"Error loading valid PatientAdverseEvent Extracts";
                Log.Error(msg);
                Log.Error($"{e}");
                return StatusCode(500, msg);
            }
        }

        [HttpGet("LoadErrors")]
        public IActionResult LoadErrors()
        {
            try
            {
                var tempPatientAdverseEventExtracts = _tempPatientAdverseEventExtractRepository.GetAll().Where(n => n.CheckError).ToList();
                return Ok(tempPatientAdverseEventExtracts);
            }
            catch (Exception e)
            {
                var msg = $"Error loading PatientAdverseEvent Extracts with errors";
                Log.Error(msg);
                Log.Error($"{e}");
                return StatusCode(500, msg);
            }
        }

        [HttpGet("LoadValidations")]
        public IActionResult LoadValidations()
        {
            try
            {
                var errorSummary = _errorSummaryRepository.GetAll()
                    .OrderByDescending(x=>x.Type)
                    .ToList();
                return Ok(errorSummary);
            }
            catch (Exception e)
            {
                var msg = $"Error loading PatientAdverseEvent error summary";
                Log.Error(msg);
                Log.Error($"{e}");
                return StatusCode(500, msg);
            }
        }
    }
}
