package pc.threading

class CancellableThread(val repeatingTask: () -> Unit) : Thread() {

    override fun run() {
        try {
            while (!currentThread().isInterrupted) {
                repeatingTask()
            }
        } catch (consumed: InterruptedException) {
            /* Allow thread to exit */
        }
    }

    fun cancel() {
        interrupt()
    }
}
