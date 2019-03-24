namespace Fasetto.Word.Core
{
    /// <summary>
    /// The result of a login request or get user profile details request via API
    /// </summary>
    public class UserProfileDetailsApiModel
    {
        #region Public Properties

        /// <summary>
        /// The authentication token used to stay authenticated through future requests
        /// </summary>
        /// <remarks>The Token is only provided when called from the login methods</remarks>
        public string Token { get; set; }

        /// <summary>
        /// The users username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The users email
        /// </summary>
        public string Email { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserProfileDetailsApiModel()
        {
            
        }

        #endregion

        #region Public Helper Methods


        #endregion
    }
}
