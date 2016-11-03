namespace StrategyIncubator
{
    class Post
    {
        public string author { get; set; }
        public string link { get; set; }
        public string summary { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(author)
                || !string.IsNullOrWhiteSpace(link)
                || !string.IsNullOrWhiteSpace(summary);
        }
    }
}
