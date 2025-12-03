using System.Text.Json;
using EcoGuardian_Backend.CRM.Application.External;
using EcoGuardian_Backend.CRM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.CRM.Domain.Model.Aggregates;
using EcoGuardian_Backend.CRM.Domain.Model.Commands;
using EcoGuardian_Backend.CRM.Domain.Repositories;
using EcoGuardian_Backend.CRM.Domain.Services;
using EcoGuardian_Backend.Shared.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace EcoGuardian_Backend.CRM.Application.Internal.CommandServices;

public class AnswerCommandService(
    IAnswerRepository answerRepository, 
    IUnitOfWork unitOfWork,
    IAddedQuestionEventHandler eventHandler, 
    IQuestionRepository questionRepository,
    IExternalUserServiceCRM externalUserService,
    IMemoryCache cache,
    IGeminiService geminiService
    ) : IAnswerCommandService
{
    public async Task Handle(RegisterAnswerCommand command)
    {
        var question = await questionRepository.GetByIdAsync(command.QuestionId);
        if (question == null)
        {
            throw new KeyNotFoundException($"Question with ID {command.QuestionId} not found.");
        }
        var prompt = "You are an expert environmental consultant. Provide a detailed and accurate answer to the following question:\n\n" +
                     $"{question.Content}\n\n" +
                     "Your answer should be clear, concise, and informative.";
        var answerContent = await geminiService.GetGeminiResponseAsync(prompt);
        var answer = new Answer(
            command.QuestionId,
            answerContent,
            1
        );
        cache.Set(
            command.QuestionId,
            answer,
            new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60)
                )
            );
        await answerRepository.AddAsync(answer);
        await unitOfWork.CompleteAsync();
        await eventHandler.HandleAnswerAddedAsync(command.QuestionId);
    }

}