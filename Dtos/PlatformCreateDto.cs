using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/*
 * DTO - Data transfer object.
 * Why we need DTO?
 * -> Actually we do not want to expose our model to the client directly.
 * -> DTO will save us from exposing direct model to the client.
 * -> So DTO is for external usage while Model is for internal usage.
 * -> DTO will save us from privacy breach.
 */

namespace PlatformService.Dtos
{
    public class PlatformCreateDto
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Publisher { get; set; }

        [Required]
        public string Cost { get; set; }
    }
}
