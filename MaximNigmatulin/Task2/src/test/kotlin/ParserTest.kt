import org.junit.jupiter.api.Test
import org.junit.jupiter.api.assertThrows
import pc.parseArgs
import java.lang.IllegalArgumentException
import java.lang.NumberFormatException

class ParserTest {

    @Test
    fun `test 0 args main`() {
        val args = emptyArray<String>()
        assertThrows<IllegalArgumentException> { parseArgs(args) }
    }

    @Test
    fun `test 1 args main`() {
        val args = arrayOf("1")
        assertThrows<IllegalArgumentException> { parseArgs(args) }
    }

    @Test
    fun `test 3 args main`() {
        val args = arrayOf("1", "2", "3")
        assertThrows<IllegalArgumentException> { parseArgs(args) }
    }

    @Test
    fun `test 2 incorrect arguments`() {
        val args = arrayOf("wrong1", "wrong2")
        assertThrows<NumberFormatException> { parseArgs(args) }
    }
}
