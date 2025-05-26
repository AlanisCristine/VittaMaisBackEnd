using Firebase.Database;

namespace VittaMais.API.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseService()
        {
            _firebaseClient = new FirebaseClient("https://vittamais-6616d-default-rtdb.firebaseio.com/");
        }

        public FirebaseClient GetDatabase()
        {
            return _firebaseClient;
        }
    }
}
