﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForumApiWebLibrary.Client.Pages
{
    public partial class Forums : ComponentBase
    {
        [CascadingParameter]
        public long FId { get; set; }
    }
}