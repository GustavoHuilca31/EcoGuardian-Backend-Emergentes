using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcoGuardian_Backend.CRM.Domain.Model.Aggregates;
using EcoGuardian_Backend.CRM.Domain.Model.ValueObjects;
using EcoGuardian_Backend.CRM.Interfaces.Rest.Resources;

namespace EcoGuardian_Backend.CRM.Interfaces.Rest.Transform
{    public class QuestionResourceFromEntityAssembler
    {        public static QuestionResource ToResourceFromEntity(Question question, string answer)
        {
            Console.WriteLine("Objeto Pregunta." + question + "Imagenes" + question.Images);

            Console.WriteLine("Imagenes pasadas a string" + question.Images?.Select(img => img.ImageUrl).ToList() );

            return new QuestionResource
            {
                QuestionId = question.Id,
                Title = question.Title,
                Content = question.Content,
                Status = ParseQuestionStateToString(question.State),
                CreatedAt = question.CreatedAt,
                UserId = question.UserId,
                UpdatedAt = question.UpdatedAt ?? DateTime.UtcNow,
                PlantId = question.PlantId,
                Answer = answer,
                ImageUrls = question.Images?.Select(img => img.ImageUrl).ToList()
            };
        }

        private static string ParseQuestionStateToString(QuestionState state)
        {
            return state switch
            {
                QuestionState.Pending => "Pending",
                QuestionState.Resolved => "Resolved",
                QuestionState.Closed => "Closed",
                _ => "Pending"
            };
        }
    }
}