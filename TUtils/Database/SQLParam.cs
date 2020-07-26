namespace TUtils.Database
{
    /// <summary>
    /// Used to expose the concept of parameterized SQL in a implementation neutral way.
    /// </summary>
    public struct SQLParam
    {
        public string Name { get; }
        public dynamic Value { get; set; }

        /// <param name="name">The name of the parameter without any preceeding characters such as '@'s.</param>
        /// <param name="value">The value of the parameter to pass in.</param>
        public SQLParam(string name, dynamic value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}