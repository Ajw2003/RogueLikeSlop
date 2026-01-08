using UnityEngine;

namespace Code.Scripts.Singleton
{ // Generic Singleton base class to ensure a single instance of a MonoBehaviour-derived class
  public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour    
  {        
  // Static reference to the singleton instance
      private static T _instance; // Property to access the singleton instance

      protected virtual bool PersistBetweenScenes => true;
      public static T Instance        
      {            
          get            
          {                
              // If the instance is not already set, create a new instance
              if (_instance == null)               
              {                    
                               
                  var singletonObject = new GameObject(typeof(T).Name);                    
                               
                  _instance = singletonObject.AddComponent<T>();                   
                               
                  DontDestroyOnLoad(singletonObject); // Make sure the singleton instance persists across scene loads   
                  
              }               
              return _instance; // return the instance
          }
      }    
        // Protected METHODS: -----------------------------------------------------------------------                
        protected virtual void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                if (PersistBetweenScenes) DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        // Clear the instance when the object is destroyed
        
  }
  
}