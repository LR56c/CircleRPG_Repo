using UnityEngine;

namespace Code.Test
{
    public class ExampleStringHash : MonoBehaviour
    {
        private readonly int codigo = "hola".GetHashCode();
        private readonly int codigo2 = "holq".GetHashCode();

        private void Awake()
        {
            Debug.Log($"codigo: {codigo.ToString()}");
            Debug.Log($"codigo2: {codigo2.ToString()}");
        }

        protected bool Equals(ExampleStringHash other)
        {
            return base.Equals(other) && codigo == other.codigo;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ExampleStringHash) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return(base.GetHashCode() * 397) ^ codigo;
            }
        }
    }
}