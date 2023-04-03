import org.junit.jupiter.api.Test
import pc.impl.DataStore
import kotlin.test.assertEquals

class DataStoreTest {

    @Test
    fun `test data is stored`() {
        val ds = DataStore<Int>()

        ds.push(1)
        ds.push(2)
        ds.push(3)
        ds.pop()

        assertEquals(ds.expose(), listOf(2, 3))
    }

    @Test
    fun `test expose`() {
        val ds = DataStore<Int>()

        ds.push(1)
        ds.push(2)
        ds.push(3)

        assertEquals(ds.expose(), listOf(1, 2, 3))
    }
}
