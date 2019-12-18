using System;
using System.Collections.Generic;

namespace MyWebsite.Model
{
    /// <summary>
    /// Defines the Story class.
    /// </summary>
    public class Story
    {
        /// <summary>
        /// Gets or sets the username of the item's author.
        /// </summary>
        /// <value>The username of the item's author..</value>
        public string By { get; set; }

        /// <summary>
        /// Gets or sets the Descendants.
        /// In the case of stories or polls, the total comment count.
        /// </summary>
        /// <value>The Descendants.</value>
        public int Descendants { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the kids.
        /// The ids of the item's comments, in ranked display order.
        /// </summary>
        /// <value>The kids.</value>
        public List<int> Kids { get; set; }

        /// <summary
        /// >Gets or sets the Score.
        /// </summary>
        /// <value>The Score.</value>
        public int Score { get; set; }

        /// <summary
        /// >Gets or sets the Time.
        /// </summary>
        /// <value>The Time.</value>
        public int Time { get; set; }

        /// <summary
        /// >Gets or sets the Title.
        /// </summary>
        /// <value>The Title.</value>
        public string Title { get; set; }

        /// <summary
        /// >Gets or sets the Type.
        /// </summary>
        /// <value>The Type.</value>
        public string Type { get; set; }

        /// <summary
        /// >Gets or sets the Url.
        /// </summary>
        /// <value>The Url.</value>
        public string Url { get; set; }
    }
}