
using EcoGuardian_Backend.CRM.Domain.Model.Aggregates;
using EcoGuardian_Backend.CRM.Domain.Model.Commands;
using EcoGuardian_Backend.CRM.Domain.Services;
using EcoGuardian_Backend.CRM.Interfaces.Rest.Resources;
using EcoGuardian_Backend.CRM.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoGuardian_Backend.CRM.Interfaces.Rest
{
    [ApiController]
    [ProducesResponseType(500)]
    [Route("api/v1/questions")]
    public class QuestionController(
        IQuestionCommandService questionCommandService,
        IQuestionQueryService questionQueryService,
        IAnswerCommandService answerCommandService,
        IAnswerQueryService answerQueryService,
        IAddedQuestionEventHandler addedQuestionEventHandler) : ControllerBase
    {

// csharp
       [HttpPost]
       [Consumes("multipart/form-data")]
       [ProducesResponseType(201)]
       [ProducesResponseType(400)]
       public async Task<IActionResult> RegisterQuestion([FromForm] RegisterQuestionCommand command)
       {
           var question = await questionCommandService.Handle(command);
           var questionResource = QuestionResourceFromEntityAssembler.ToResourceFromEntity(question, string.Empty);
           return Ok(questionResource);
       }
       
       [HttpPut]
       [ProducesResponseType(200)]
       [ProducesResponseType(400)]
       public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionCommand command)
       {
           await questionCommandService.Handle(command);
           var question = await questionQueryService.GetQuestionById(command.QuestionId);
       
           var answer = await answerQueryService.GetAnswerByQuestionId(question.Id);
           var answerContent = answer?.Content ?? string.Empty;
       
           var updatedQuestion = QuestionResourceFromEntityAssembler.ToResourceFromEntity(question, answerContent);
           return Ok(updatedQuestion);
       }
       
       [HttpGet("{questionId:int}")]
       [ProducesResponseType(200)]
       [ProducesResponseType(404)]
       public async Task<IActionResult> GetQuestionById(int questionId)
       {
           var question = await questionQueryService.GetQuestionById(questionId);
       
           var answer = await answerQueryService.GetAnswerByQuestionId(questionId);
           var answerContent = answer?.Content ?? string.Empty;
       
           var questionResource = QuestionResourceFromEntityAssembler.ToResourceFromEntity(question, answerContent);
           return Ok(questionResource);
       }
       
       // By uSer ID
       [HttpGet("user/{userId:int}")]
       [ProducesResponseType(200)]
       [ProducesResponseType(404)]
       public async Task<IActionResult> GetQuestionsByUserId(int userId)
       {
           var questions = (await questionQueryService.GetQuestionsByUserId(userId))?.ToList();
           if (questions == null || questions.Count == 0)
           {
               return NotFound();
           }
       
           var answerTasks = questions.Select(q => answerQueryService.GetAnswerByQuestionId(q.Id)).ToArray();
           var answers = await Task.WhenAll(answerTasks);
       
           var questionsResource = new List<QuestionResource>(questions.Count);
           for (int i = 0; i < questions.Count; i++)
           {
               var q = questions[i];
               var answerContent = answers[i]?.Content ?? string.Empty;
               questionsResource.Add(QuestionResourceFromEntityAssembler.ToResourceFromEntity(q, answerContent));
           }
       
           return Ok(questionsResource);
       }
       
        // By Plant ID
// csharp
        [HttpGet("plant/{plantId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetQuestionsByPlantId(int plantId)
        {
            var questions = (await questionQueryService.GetQuestionsByPlantId(plantId))?.ToList();
        
            if (questions == null || questions.Count == 0)
            {
                return NotFound();
            }
        
            // Fetch all answers in parallel preserving the order
            var answerTasks = questions.Select(q => answerQueryService.GetAnswerByQuestionId(q.Id)).ToArray();
            var answers = await Task.WhenAll(answerTasks);
        
            var resources = new List<QuestionResource>(questions.Count);
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var answerContent = answers[i]?.Content ?? string.Empty;
                var resource = QuestionResourceFromEntityAssembler.ToResourceFromEntity(question, answerContent);
                resources.Add(resource);
            }
        
            return Ok(resources);
        }
        
        [HttpGet("{questionId:int}/answers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAnswersByQuestionId(int questionId)
        {
            var answers = await answerQueryService.GetAnswerByQuestionId(questionId);
            var question = await questionQueryService.GetQuestionById(questionId);
            var answersResource = AnswerResourceFromEntityAssembler.FromEntity(answers, question);  
            return Ok(answersResource);
        }

        [HttpPost("{questionId:int}/answers")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegisterAnswer(int questionId, [FromBody] RegisterAnswerResource resource)
        {
            var command = RegisterAnswerCommandFromResourceAssembler.toCommandFromResource(resource, questionId);

            await answerCommandService.Handle(command);
            await addedQuestionEventHandler.HandleAnswerAddedAsync(questionId);

            var answer = await answerQueryService.GetAnswerByQuestionId(questionId);

            var question = await questionQueryService.GetQuestionById(questionId);
            var answersResource = AnswerResourceFromEntityAssembler.FromEntity(answer, question);
            return Ok(answersResource);
        }

        //Get all questions
[HttpGet]
      [ProducesResponseType(200)]
      [ProducesResponseType(404)]
      public async Task<IActionResult> GetAllQuestions()
      {
          var questions = (await questionQueryService.GetAllQuestions())?.ToList();
          if (questions == null || questions.Count == 0)
          {
              return Ok(new List<QuestionResource>());
          }
      
          var answerTasks = questions.Select(q => answerQueryService.GetAnswerByQuestionId(q.Id)).ToArray();
          var answers = await Task.WhenAll(answerTasks);
      
          var resources = new List<QuestionResource>(questions.Count);
          for (int i = 0; i < questions.Count; i++)
          {
              var q = questions[i];
              var answerContent = answers[i]?.Content ?? string.Empty;
              resources.Add(QuestionResourceFromEntityAssembler.ToResourceFromEntity(q, answerContent));
          }
      
          return Ok(resources);
      }
      
        
        [HttpGet("answers/specialist")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSpecialistAnswers([FromQuery] int specialistId)
        {
            var answers = await answerQueryService.GetAnswersBySpecialistId(specialistId);
            var enumerable = answers as Answer[] ?? answers.ToArray();
            if (enumerable.Length == 0)
            {
                return Ok(new List<AnswerResource>());
            }

            List<AnswerResource?> answerResource = [];
            foreach (var answer in enumerable)
            {
                var question = await questionQueryService.GetQuestionById(answer.QuestionId);
                {
                    var answerResourceItem = AnswerResourceFromEntityAssembler.FromEntity(answer, question);
                    answerResource.Add(answerResourceItem);
                }
            }
            return Ok(answerResource);
        }
        

    }
}