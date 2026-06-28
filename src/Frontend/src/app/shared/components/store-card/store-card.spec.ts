import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreCard } from './store-card';

describe('StoreCard', () => {
  let component: StoreCard;
  let fixture: ComponentFixture<StoreCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoreCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoreCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
