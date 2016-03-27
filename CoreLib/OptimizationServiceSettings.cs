using System.Runtime.Serialization;

namespace CoreLib
{
    [DataContract]
    public class OptimizationServiceSettings
    {
      //  public string ErrorMessage { get; set; }

        [DataMember]
        public bool StealthMode { get; set; }

        public OptimizationServiceSettings()
        {
            
        }

        //public OptimizationServiceSettings(SerializationInfo info, StreamingContext context)
        //{
        //    if (info != null)
        //        this.ErrorMessage = info.GetString("ErrorMessage");
        //}
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    if (info != null)
        //        info.AddValue("ErrorMessage", this.ErrorMessage);
        //}
    }
}
