namespace StrategyIncubator
{
    class Post
    {
        public string title { get; set; }
        public string author { get; set; }
        public string link { get; set; }
        public string summary { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(title)
                || !string.IsNullOrWhiteSpace(author)
                || !string.IsNullOrWhiteSpace(link)
                || !string.IsNullOrWhiteSpace(summary);
        }
    }
}
