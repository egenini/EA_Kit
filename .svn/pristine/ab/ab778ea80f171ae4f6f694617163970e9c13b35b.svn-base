namespace DMN.dominio
{
    public class CondicionValue : Variable
    {
        public CondicionValue(string valor, Condition condition)
        {
            this.value = valor == "-" ? "" : valor;
            this.attributeName = condition.attributeName;
            this.businessName = condition.businessName;
            this.dataType = condition.dataType;
        }
    }
}