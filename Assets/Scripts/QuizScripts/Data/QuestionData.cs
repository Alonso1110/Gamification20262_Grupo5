using System;

[Serializable]
public class QuestionData
{
    public string questionText;
    public string[] answers; 
}

[Serializable]
public class TriviaDatabase
{
    public QuestionData[] questions;
}
