namespace DMN.dominio
{
    public class ConclusionValue : Variable
    {
        public ConclusionValue(string valor, Conclusion conclusion)
        {
            this.value = valor;
            this.attributeName = conclusion.attributeName;
            this.businessName = conclusion.businessName;
            this.dataType = conclusion.dataType;
        }
    }
}