import 'package:flutter/material.dart';
import 'package:frontend/widgets/box_widget.dart';
import 'package:lottie/lottie.dart';
import 'package:provider/provider.dart';
import '../data/quiz_data_source.dart';
import '../models/entities.dart';

class QuizListWidget extends StatefulWidget {
  const QuizListWidget({super.key});

  @override
  _QuizListWidgetState createState() => _QuizListWidgetState();
}

class _QuizListWidgetState extends State<QuizListWidget> {

  @override
  Widget build(BuildContext context) {
    final isSmallScreen = MediaQuery.of(context).size.width < 600;


    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(height: isSmallScreen ? 30 : 10),
          _buildQuizzes(context,
              context.read<QuizDataSource>().getPopularQuizzes(), 'Most Popular', 5),
          const SizedBox(height: 40),
          _buildQuizzes(context,
              context.read<QuizDataSource>().getNewestQuizzes(), 'Newest', 10),
        ],
      ),
    );
  }

  Widget _buildQuizzes(
      BuildContext context, Future<List<Quiz>> future, String title, int itemCountLimit) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: Text(
            title,
            style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
        ),
        const SizedBox(height: 10),
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            children: [
              FutureBuilder<List<Quiz>>(
                future: future,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return Center(
                      child: Lottie.asset(
                        'assets/animations/loading.json',
                        height: 50,
                        width: 50,
                      ),
                    );
                  } else if (snapshot.hasError) {
                    return Center(child: Text('Error: ${snapshot.error}'));
                  } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                    return const Center(child: Text('No quizzes available'));
                  } else {
                    final itemCount = snapshot.data!.length > itemCountLimit
                        ? itemCountLimit
                        : snapshot.data!.length;

                    return Row(
                      children: List.generate(itemCount, (index) {
                        final Quiz quiz = snapshot.data![index];
                        return Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: BoxWidget(
                            quiz: quiz,
                          ),
                        );
                      }),
                    );
                  }
                },
              ),
            ],
          ),
        ),
      ],
    );
  }
}
