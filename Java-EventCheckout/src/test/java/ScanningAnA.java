import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class ScanningAnA {
    @Test
    public void causesPriceToBeFifty() {
        Checkout checkout = new Checkout();
        checkout.apply(new ItemScanned("A"));
        assertEquals(50, checkout.total());
    }

}
