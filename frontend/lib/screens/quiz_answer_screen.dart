import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/bloc/quiz_bloc.dart';
import 'package:frontend/bloc/quiz_state.dart';
import 'package:frontend/models/events.dart';
import 'package:frontend/screens/quiz_victory_screen.dart';

class AnswerButton extends StatelessWidget {
  final Color color;
  final IconData iconData;
  final String answerText;
  final VoidCallback onTap;

  const AnswerButton({
    super.key,
    required this.color,
    required this.iconData,
    required this.answerText,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(
          color: color,
          borderRadius: BorderRadius.circular(5.0),
        ),
        margin: const EdgeInsets.all(10.0),
        width: 180.0,
        height: 400.0,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              iconData,
              color: Colors.white,
            ),
            Text(
              answerText,
              textAlign: TextAlign.center, // Add this line
              style: const TextStyle(
                color: Colors.white,
                fontSize: 24.0,
              ),
            )
          ],
        ),
      ),
    );
  }
}

class AnswerScreen extends StatelessWidget {
  final List<Color> buttonColors = [
    Colors.blue,
    Colors.red,
    Colors.green,
    Colors.yellow
  ];
  final List<IconData> buttonIcons = [
    Icons.square,
    Icons.favorite,
    Icons.circle,
    Icons.star
  ];

  AnswerScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(''),
        backgroundColor: Colors.indigo[300],
        automaticallyImplyLeading: false,
      ),
      body: BlocBuilder<QuizBloc, QuizState>(
        builder: (context, state) {
          if (state.scores.isNotEmpty) {
            // Navigate to VictoryScreen
            WidgetsBinding.instance!.addPostFrameCallback((_) {
              Navigator.pushReplacement(
                context,
                MaterialPageRoute(
                    builder: (context) => const QuizVictoryScreen()),
              );
            });
          }
          if (state.answerButtonPressed) {
            return Container();
          }
          return Padding(
              padding: const EdgeInsets.all(8.0),
              child: GridView.count(
                  crossAxisCount: 2,
                  // This creates 2 columns
                  childAspectRatio: 1.0,
                  // Adjust this value to change how square/circular the buttons are
                  mainAxisSpacing: 10.0,
                  // Adjust this value for the desired spacing
                  crossAxisSpacing: 10.0,
                  // Adjust this value for the desired spacing
                  children: List.generate(
                    state.answersForCurrentQuestion.length,
                    (i) => AnswerButton(
                      color: buttonColors[i % buttonColors.length],
                      iconData: buttonIcons[i % buttonIcons.length],
                      answerText: state.answersForCurrentQuestion[i].text,
                      onTap: () {
                        context.read<QuizBloc>().clientWantsToAnswerQuestion(
                              state.answersForCurrentQuestion[i].id,
                              state.username,
                              // Replace with the actual username
                              state.roomId, // Replace with the actual room id
                            );
                        context
                            .read<QuizBloc>()
                            .add(ClientEvent.clientWantsToAnswerQuestion(
                              answerId: state.answersForCurrentQuestion[i].id,
                              username: state.username,
                              roomId: state.roomId,
                            ));
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text(
                                'You answered: ${state.answersForCurrentQuestion[i].text}'),
                            duration: const Duration(seconds: 2),
                          ),
                        );
                      },
                    ),
                  )));
        },
      ),
    );
  }
}
