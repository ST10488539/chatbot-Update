using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public string[] Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }

        public QuizQuestion(string text, string[] options, string answer, string explanation)
        {
            QuestionText = text;
            Options = options;
            CorrectAnswer = answer.ToUpper();
            Explanation = explanation;
        }
    }

    public class ResponseHandler
    {
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private int currentQuestionIndex = -1;
        private int quizScore = 0;
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private string currentConversationState = "AWAITING_NAME";
        private string pendingTaskTitle = "";
        private string userName = "";
        private List<string> activityLog = new List<string>();

        public ResponseHandler()
        {
            InitializeQuiz();
            LogAction("Chatbot initialized and system modules loaded.");
        }

        private void LogAction(string description)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            activityLog.Add($"[{timestamp}] {description}");
        }

        public void InitializeQuiz()
        {
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion("What should you do if you receive an email asking for your password?",
                    new string[]{"A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it"},
                    "C", "Reporting phishing emails helps security teams block threats permanently."),

                new QuizQuestion("Two-Factor Authentication (2FA) makes accounts significantly more secure.",
                    new string[]{"True", "False"},
                    "True", "Even if a hacker steals your password, they cannot log in without your secondary code."),

                new QuizQuestion("Which of the following is considered a strong password?",
                    new string[]{"A) Password123!", "B) Blue$Sky#99!22", "C) Admin2026", "D) Qwerty!!!!"},
                    "B", "Strong passwords use a mix of uppercase, lowercase, numbers, symbols, and are at least 12 characters long."),

                new QuizQuestion("Public Wi-Fi networks (like at a coffee shop) are completely safe for checking bank accounts.",
                    new string[]{"True", "False"},
                    "False", "Public Wi-Fi networks can be easily intercepted by bad actors using packet sniffers."),

                new QuizQuestion("What is 'Phishing'?",
                    new string[]{"A) Optimizing computer database storage", "B) Sending fraudulent emails to trick individuals into revealing sensitive info", "C) A method of scanning networks for open ports", "D) Updating your operating system software"},
                    "B", "Phishing mimics real companies to manipulate you into giving away credentials or data."),

                new QuizQuestion("If your computer slows down out of nowhere and pop-ups appear, it might be infected with malware.",
                    new string[]{"True", "False"},
                    "True", "Sudden performance drops and spammy behavior are classic indicators of malicious software infections."),

                new QuizQuestion("What does the 'S' stand for in HTTPS when looking at a website link?",
                    new string[]{"A) System", "B) Secure", "C) Speed", "D) Shareable"},
                    "B", "HTTPS means data traveling between your browser and the website server is encrypted securely."),

                new QuizQuestion("Antivirus software only needs to be updated once a year.",
                    new string[]{"True", "False"},
                    "False", "New cyber threats emerge daily, so antivirus definitions require constant, automated updates."),

                new QuizQuestion("What is 'Social Engineering'?",
                    new string[]{"A) Writing code for social media applications", "B) Reconfiguring local network hardware routers", "C) Manipulating people into giving up confidential information", "D) Installing security patches on windows"},
                    "C", "Social engineering attacks exploit human psychology and trust rather than technical system loopholes."),

                new QuizQuestion("Using the exact same password across multiple online accounts is safe as long as it's a complex password.",
                    new string[]{"True", "False"},
                    "False", "If one website suffers a data breach, hackers will immediately try those leaked credentials on every other platform."),

                new QuizQuestion("What is ransomware?",
                    new string[]{"A) Software that blocks access to a system until money is paid", "B) An application that speeds up memory storage processing", "C) A tool used to monitor background network metrics", "D) A type of browser cookie extension"},
                    "A", "Ransomware encrypts your files and holds them hostage demanding financial payments to unlock them.")
            };
        }

        public string GetResponse(string input)
        {
            string cleanInput = input.Trim();
            string upperInput = cleanInput.ToUpper();

            if (currentConversationState == "AWAITING_NAME")
            {
                if (string.IsNullOrEmpty(cleanInput))
                {
                    return "Please enter a valid name so I know who I am assisting!";
                }

                userName = cleanInput;
                currentConversationState = "NONE";
                LogAction($"User identity established: {userName}.");

                return $"Nice to meet you, {userName}! 👋 I am SideKick, your cybersecurity digital assistant.\n\n" +
                       $"Here is what you can do now:\n" +
                       $"🔹 Type `start quiz` to test your online safety knowledge.\n" +
                       $"🔹 Type `Remind me to update my password` to log a security task.\n" +
                       $"🔹 Type `show activity log` to see everything we've done.";
            }

            if (upperInput.Contains("START QUIZ") || upperInput.Contains("PLAY GAME") || upperInput.Contains("CYBER QUIZ") || (upperInput.Contains("QUIZ") && currentQuestionIndex == -1))
            {
                currentConversationState = "NONE";
                currentQuestionIndex = 0;
                quizScore = 0;

                LogAction($"{userName} started the quiz game module.");
                var firstQ = quizQuestions[currentQuestionIndex];
                return $"🎮 **Welcome to the Cybersecurity Mini-Game, {userName}!**\nLet's see how safe you are online. Answer with the correct option letter or True/False.\n\n" +
                       $"**Question 1:** {firstQ.QuestionText}\n\n" + string.Join("\n", firstQ.Options);
            }

            if (currentQuestionIndex >= 0 && currentQuestionIndex < quizQuestions.Count)
            {
                var currentQ = quizQuestions[currentQuestionIndex];
                string userFeedback = "";

                if (upperInput == currentQ.CorrectAnswer ||
                    (currentQ.CorrectAnswer == "TRUE" && upperInput == "T") ||
                    (currentQ.CorrectAnswer == "FALSE" && upperInput == "F"))
                {
                    quizScore++;
                    userFeedback = "🎉 **Correct!** " + currentQ.Explanation;
                }
                else
                {
                    userFeedback = "❌ **Incorrect.** The right answer was **" + currentQ.CorrectAnswer + "**. " + currentQ.Explanation;
                }

                currentQuestionIndex++;

                if (currentQuestionIndex < quizQuestions.Count)
                {
                    var nextQ = quizQuestions[currentQuestionIndex];
                    return $"{userFeedback}\n\n----------------------------------------\n\n" +
                           $"**Question {currentQuestionIndex + 1}:** {nextQ.QuestionText}\n\n" + string.Join("\n", nextQ.Options);
                }
                else
                {
                    int finalScore = quizScore;
                    currentQuestionIndex = -1;

                    LogAction($"Quiz session completed by {userName}. Score: {finalScore}/{quizQuestions.Count}.");
                    string performanceText = finalScore >= 8
                        ? $"🏆 Great job, {userName}! You're a cybersecurity pro! Your data hygiene is top tier."
                        : $"🛡️ Keep learning to stay safe online, {userName}! Cyber awareness takes ongoing practice.";

                    return $"{userFeedback}\n\n==============================\n" +
                           $"🏁 **Quiz Completed!**\n" +
                           $"Your Final Score: **{finalScore} / {quizQuestions.Count}**\n\n" +
                           $"{performanceText}\n\nType `start quiz` to try again or ask me a standard safety question!";
                }
            }

            if (currentConversationState == "AWAITING_REMINDER_DAYS")
            {
                int days = 0;
                string numericPart = new string(cleanInput.Where(char.IsDigit).ToArray());

                if (int.TryParse(numericPart, out days))
                {
                    bool success = dbHelper.AddTask(pendingTaskTitle, days);
                    currentConversationState = "NONE";

                    if (success)
                    {
                        LogAction($"Task saved: '{pendingTaskTitle}' with a {days}-day reminder frequency.");
                        pendingTaskTitle = "";
                        return $"✅ Task successfully saved to database with a {days}-day reminder frequency status update!";
                    }
                    else
                    {
                        pendingTaskTitle = "";
                        return "❌ Internal failure tracking connectivity. Could not record entry directly into MySQL database.";
                    }
                }
                else if (upperInput == "NO" || upperInput == "NONE" || upperInput == "0")
                {
                    bool success = dbHelper.AddTask(pendingTaskTitle, 0);
                    currentConversationState = "NONE";

                    if (success)
                    {
                        LogAction($"Task saved: '{pendingTaskTitle}' with no active reminders.");
                        pendingTaskTitle = "";
                        return "✅ Task successfully saved to database without any automated reminder constraints.";
                    }
                    else
                    {
                        pendingTaskTitle = "";
                        return "❌ Internal failure tracking connectivity. Could not record entry directly into MySQL database.";
                    }
                }
                return "Please specify a numeric format sequence (e.g., `3 days`) or state `no` if no alerts are desired.";
            }

            if (upperInput.Contains("SHOW ACTIVITY LOG") || upperInput.Contains("VIEW LOG") || upperInput.Contains("ACTIVITY LOG") || upperInput.Contains("WHAT HAVE YOU DONE FOR ME"))
            {
                LogAction("Activity log requested.");

                var recentActions = activityLog.AsEnumerable().Reverse().Take(10).Reverse().ToList();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine($"📋 **Here's a summary of recent actions for {userName}:**\n");

                for (int i = 0; i < recentActions.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {recentActions[i]}");
                }

                return sb.ToString();
            }

            if (upperInput.Contains("VIEW TASKS") || upperInput.Contains("SHOW TASKS") || upperInput.Contains("LIST TASKS"))
            {
                LogAction("Database security tasks list pulled.");
                return dbHelper.GetAllTasksFormatted();
            }

            if (upperInput.Contains("ADD TASK") || upperInput.Contains("SET REMINDER") || upperInput.Contains("REMIND ME TO") || upperInput.Contains("ADD A TASK"))
            {
                string extractedTask = cleanInput;
                string[] triggers = new string[] { "ADD TASK TO", "ADD TASK -", "ADD TASK", "SET REMINDER TO", "SET REMINDER", "REMIND ME TO", "ADD A TASK TO", "ADD A TASK" };

                foreach (var trigger in triggers)
                {
                    int index = extractedTask.ToUpper().IndexOf(trigger);
                    if (index != -1)
                    {
                        extractedTask = extractedTask.Substring(index + trigger.Length).Trim();
                        break;
                    }
                }

                if (extractedTask.ToUpper().EndsWith("TOMORROW"))
                {
                    extractedTask = extractedTask.Substring(0, extractedTask.Length - 8).Trim();
                }

                if (string.IsNullOrEmpty(extractedTask))
                {
                    return "Please state a valid structural context label parameter to map an operational task.";
                }

                pendingTaskTitle = extractedTask;
                currentConversationState = "AWAITING_REMINDER_DAYS";

                LogAction($"NLP Match: Task intent detected. Title: '{pendingTaskTitle}'.");
                return $"Task added: '{pendingTaskTitle}'. Would you like to set a reminder for this task? Please provide the number of days or type `no`.";
            }

            if (upperInput.Contains("HELLO") || upperInput.Contains("HI"))
            {
                return $"Hello again, {userName}! I am SideKick, your cybersecurity digital assistant tracker. Try typing `start quiz` or ask me to set a task reminder.";
            }

            return $"I didn't quite understand that, {userName}. Could you rephrase? You can say things like 'Remind me to update my password', 'start quiz', or 'show activity log'.";
        }
    }
}
