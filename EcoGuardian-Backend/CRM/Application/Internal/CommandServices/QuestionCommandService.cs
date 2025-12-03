using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EcoGuardian_Backend.CRM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.CRM.Domain.Model.Aggregates;
using EcoGuardian_Backend.CRM.Domain.Model.Commands;
using EcoGuardian_Backend.CRM.Domain.Repositories;
using EcoGuardian_Backend.CRM.Domain.Services;
using EcoGuardian_Backend.Shared.Application.Internal.CloudinaryStorage;
using EcoGuardian_Backend.Shared.Domain.Repositories;

namespace EcoGuardian_Backend.CRM.Application.Internal.CommandServices;

public class QuestionCommandService(
    IQuestionRepository questionRepository, 
    IUnitOfWork unitOfWork, 
    IExternalUserServiceCRM externalUserService, 
    ICloudinaryStorage cloudinaryStorage,
    ILogger<QuestionCommandService> logger
    ) : IQuestionCommandService
{
    public async Task<Question> Handle(RegisterQuestionCommand command)
    {
        logger.LogInformation("RegisterQuestionCommand started. UserId={UserId}, PlantId={PlantId}, Title={Title}", command.UserId, command.PlantId, command.Title);

        // Check if the user exists before adding the question
        var userId = await externalUserService.CheckUserExists(command.UserId);
        logger.LogDebug("User existence checked. RequestedUserId={RequestedUserId}, ResolvedUserId={ResolvedUserId}", command.UserId, userId);

// Check if the plant exists
        if (!await externalUserService.CheckPlantExists(command.PlantId))
        {
            logger.LogWarning("Plant not found. PlantId={PlantId}", command.PlantId);
            throw new BadHttpRequestException($"Plant with ID {command.PlantId} does not exist.");
        }
        logger.LogDebug("Plant exists. PlantId={PlantId}", command.PlantId);

        var question = new Question(
            command.Title,
            command.Content,
            userId,
            command.PlantId
        );
        if (command.ImageUrls != null && command.ImageUrls.Any())
        {
            logger.LogDebug("Processing {ImageCount} image(s) for question by UserId={UserId}", command.ImageUrls.Count, command.UserId);
            var imageUrls = new List<string>();
            foreach (var image in command.ImageUrls)
            {
                var publicId = $"{command.UserId}/{command.PlantId}/{Guid.NewGuid()}";
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, image.OpenReadStream()),
                    PublicId = publicId,
                    Overwrite = true,
                    AllowedFormats = new[] { "jpg", "png", "gif", "webp" },
                };
                logger.LogDebug("Uploading image. FileName={FileName}, PublicId={PublicId}", image.FileName, publicId);
                await cloudinaryStorage.UploadImage(uploadParams);
                var result = await cloudinaryStorage.GetImage(publicId);
                logger.LogDebug("Image upload result. PublicId={PublicId}, Url={Url}", publicId, result ?? string.Empty);
                imageUrls.Add(result ?? string.Empty);
            }
            question.AddImages(imageUrls);
            logger.LogInformation("Added {Count} image URLs to question", imageUrls.Count);
        }
        await questionRepository.AddAsync(question);
        logger.LogDebug("Question added to repository (pending commit). Title={Title}", command.Title);
        await unitOfWork.CompleteAsync();
        logger.LogInformation("RegisterQuestionCommand completed successfully. Title={Title}", command.Title);
        return question;
    }

    public async Task Handle(UpdateQuestionCommand command)
    {
        logger.LogInformation("UpdateQuestionCommand started. QuestionId={QuestionId}, NewState={State}", command.QuestionId, command.State);
        var question = await questionRepository.GetByIdAsync(command.QuestionId);
        if (question == null)
        {
            logger.LogWarning("Question not found. QuestionId={QuestionId}", command.QuestionId);
            throw new BadHttpRequestException($"Question with ID {command.QuestionId} not found.");
        }

        logger.LogDebug("Updating question state. QuestionId={QuestionId}, FromState={OldState}, ToState={NewState}", command.QuestionId, question.State, command.State);
        question.UpdateState(command.State);
        questionRepository.Update(question);
        await unitOfWork.CompleteAsync();
        logger.LogInformation("UpdateQuestionCommand completed successfully. QuestionId={QuestionId}", command.QuestionId);
    }
}