using System;

namespace MyWebsite.Dtos
{
    public partial class StoryDto
    {
        /// <summary
        /// >Gets or sets the Title.
        /// </summary>
        /// <value>The Title.</value>
        public string Title { get; set; }

        /// <summary
        /// >Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; set; }

        /// <summary
        /// >Gets or sets the posted by.
        /// </summary>
        /// <value>The posted by.</value>
        public string PostedBy { get; set; }

        /// <summary
        /// >Gets or sets the Time.
        /// </summary>
        /// <value>The Time.</value>
        public DateTime Time { get; set; }

        /// <summary
        /// >Gets or sets the Score.
        /// </summary>
        /// <value>The Score.</value>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the comment count.
        /// </summary>
        /// <value>The comment count.</value>
        public int CommentCount { get; set; }
    }
}