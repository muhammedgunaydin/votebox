namespace vote_box.Models
{
    public class VoteModel
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string UserId { get; set; }
        public int OptionId { get; set; }
        public PollModel Poll { get; set; }
        public OptionModel Option { get; set; }
    }
}