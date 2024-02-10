namespace vote_box.Models
{
    public class OptionModel
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; }
        public int VoteCount { get; set; }
        public PollModel Poll { get; set; }
    }
}